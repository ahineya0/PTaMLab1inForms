using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiLinkedLists
{
    public enum ComponentType : byte
    {
        Изделие = 1,
        Узел = 2,
        Деталь = 3
    }

    public class Component
    {
        public int Offset { get; set; }
        public string Name { get; set; }
        public ComponentType Type { get; set; }
        public bool IsDeleted { get; set; }
        public int SpecHead { get; set; }
        public int Next { get; set; }
    }

    public class SpecRecord
    {
        public int Offset { get; set; }
        public int CompOffset { get; set; }
        public short Count { get; set; }
        public int Next { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FileManager
    {
        private static FileManager _instance;
        public static FileManager Instance => _instance ??= new FileManager();

        private FileStream _compFs;
        private BinaryReader _compReader;
        private BinaryWriter _compWriter;
        private FileStream _specFs;
        private BinaryReader _specReader;
        private BinaryWriter _specWriter;

        public string CurrentFile { get; private set; }
        public string SpecFile { get; private set; }
        public bool IsOpen => _compFs != null;

        private FileManager() { }

        public static bool CreateNew(string filename, short maxLength, string prsFilename)
        {
            filename = Path.ChangeExtension(filename, ".prd");
            prsFilename = Path.ChangeExtension(prsFilename, ".prs");

            if (File.Exists(filename))
            {
                using var fs = new FileStream(filename, FileMode.Open);
                using var br = new BinaryReader(fs);
                if (fs.Length >= 2)
                {
                    byte b1 = br.ReadByte(), b2 = br.ReadByte();
                    if (b1 == 'P' && b2 == 'S')
                    {
                        var r = System.Windows.Forms.MessageBox.Show(
                            $"Файл '{filename}' уже существует. Перезаписать?",
                            "Подтверждение",
                            System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Question);
                        if (r != System.Windows.Forms.DialogResult.Yes)
                            return false;
                    }
                }
            }

            using (var fs = new FileStream(prsFilename, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(-1);
                bw.Write(8);
            }

            using (var fs = new FileStream(filename, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write((byte)'P');
                bw.Write((byte)'S');
                bw.Write(maxLength);
                bw.Write(-1);
                bw.Write(28);
                byte[] nameBytes = new byte[16];
                var src = Encoding.ASCII.GetBytes(prsFilename);
                Array.Copy(src, nameBytes, Math.Min(src.Length, 16));
                bw.Write(nameBytes);
            }
            return true;
        }

        public void Open(string filename)
        {
            Close();
            filename = Path.ChangeExtension(filename, ".prd");

            _compFs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            _compReader = new BinaryReader(_compFs);
            _compWriter = new BinaryWriter(_compFs);

            if (_compReader.ReadByte() != 'P' || _compReader.ReadByte() != 'S')
            {
                Close();
                throw new Exception("Неверная сигнатура файла.");
            }

            _compReader.ReadInt16();
            _compReader.ReadInt32();
            _compReader.ReadInt32();

            byte[] nameBytes = _compReader.ReadBytes(16);
            string prsFromHeader = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');

            if (string.IsNullOrWhiteSpace(prsFromHeader) || !File.Exists(prsFromHeader))
                prsFromHeader = Path.ChangeExtension(filename, ".prs");

            if (!File.Exists(prsFromHeader))
            {
                Close();
                throw new Exception("Файл спецификаций не найден.");
            }

            SpecFile = prsFromHeader;
            _specFs = new FileStream(SpecFile, FileMode.Open, FileAccess.ReadWrite);
            _specReader = new BinaryReader(_specFs);
            _specWriter = new BinaryWriter(_specFs);
            CurrentFile = filename;
        }

        public void Close()
        {
            _compReader?.Close(); _compWriter?.Close(); _compFs?.Close();
            _compFs = null; _compReader = null; _compWriter = null;
            _specReader?.Close(); _specWriter?.Close(); _specFs?.Close();
            _specFs = null; _specReader = null; _specWriter = null;
        }

        private short GetMaxLen()
        {
            _compFs.Seek(2, SeekOrigin.Begin);
            return _compReader.ReadInt16();
        }

        private int GetHead()
        {
            _compFs.Seek(4, SeekOrigin.Begin);
            return _compReader.ReadInt32();
        }

        public List<Component> GetAllComponents(bool includeDeleted = false)
        {
            var result = new List<Component>();
            _compFs.Seek(2, SeekOrigin.Begin);
            short len = _compReader.ReadInt16();
            int head = _compReader.ReadInt32();

            while (head != -1)
            {
                _compFs.Seek(head, SeekOrigin.Begin);
                byte del = _compReader.ReadByte();
                int spec = _compReader.ReadInt32();
                int next = _compReader.ReadInt32();
                byte type = _compReader.ReadByte();
                string name = new string(_compReader.ReadChars(len)).Trim('\0', ' ');

                if (includeDeleted || del == 0)
                    result.Add(new Component
                    {
                        Offset = head, Name = name,
                        Type = (ComponentType)type,
                        IsDeleted = del != 0,
                        SpecHead = spec, Next = next
                    });
                head = next;
            }
            return result;
        }

        public void AddComponent(string name, ComponentType type)
        {
            _compFs.Seek(2, SeekOrigin.Begin);
            short maxLen = _compReader.ReadInt16();
            int head = _compReader.ReadInt32();
            int free = _compReader.ReadInt32();

            _compFs.Seek(free, SeekOrigin.Begin);
            int offset = (int)_compFs.Position;

            _compWriter.Write((byte)0);
            _compWriter.Write(-1);
            _compWriter.Write(head);
            _compWriter.Write((byte)type);
            byte[] data = new byte[maxLen];
            byte[] src = Encoding.ASCII.GetBytes(name);
            int copyLen = Math.Min(src.Length, maxLen);
            Array.Copy(src, data, copyLen);
            for (int i = copyLen; i < maxLen; i++) data[i] = (byte)' ';
            _compWriter.Write(data);

            _compFs.Seek(4, SeekOrigin.Begin);
            _compWriter.Write(offset);
            _compFs.Seek(8, SeekOrigin.Begin);
            _compWriter.Write(offset + 1 + 4 + 4 + 1 + maxLen);
            _compFs.Flush();
        }

        public void UpdateComponent(int offset, string name, ComponentType type)
        {
            _compFs.Seek(2, SeekOrigin.Begin);
            short maxLen = _compReader.ReadInt16();

            _compFs.Seek(offset + 1 + 4 + 4, SeekOrigin.Begin);
            _compWriter.Write((byte)type);
            byte[] data = new byte[maxLen];
            byte[] src = Encoding.ASCII.GetBytes(name);
            int copyLen = Math.Min(src.Length, maxLen);
            Array.Copy(src, data, copyLen);
            for (int i = copyLen; i < maxLen; i++) data[i] = (byte)' ';
            _compWriter.Write(data);
            _compFs.Flush();
        }

        public void DeleteComponent(int offset)
        {
            if (IsComponentReferenced(offset))
                throw new Exception("Невозможно удалить: компонент используется в спецификациях.");

            _compFs.Seek(offset + 1, SeekOrigin.Begin);
            int specHead = _compReader.ReadInt32();

            _compFs.Seek(offset, SeekOrigin.Begin);
            _compWriter.Write((byte)1);

            if (specHead != -1)
                DeleteSpecChain(specHead);

            _compFs.Flush();
            _specFs?.Flush();
        }

        public void RestoreComponent(int offset)
        {
            _compFs.Seek(offset, SeekOrigin.Begin);
            _compWriter.Write((byte)0);

            _compFs.Seek(offset + 1, SeekOrigin.Begin);
            int specHead = _compReader.ReadInt32();
            if (specHead != -1)
                RestoreSpecChain(specHead);

            _compFs.Flush();
        }

        public void RestoreAll()
        {
            var all = GetAllComponents(true);
            foreach (var c in all)
            {
                if (c.IsDeleted)
                    RestoreComponent(c.Offset);
            }
        }

        public void Truncate()
        {
            _compFs.Seek(2, SeekOrigin.Begin);
            short len = _compReader.ReadInt16();
            int recordSize = 1 + 4 + 4 + 1 + len;
            int specRecordSize = 1 + 4 + 2 + 4;

            var order = new List<int>();
            var compMap = new Dictionary<int, int>();
            int cur = GetHead();
            while (cur != -1)
            {
                _compFs.Seek(cur, SeekOrigin.Begin);
                byte del = _compReader.ReadByte();
                _compReader.ReadInt32();
                int next = _compReader.ReadInt32();
                _compReader.ReadByte();
                _compReader.ReadBytes(len);
                if (del == 0) order.Add(cur);
                cur = next;
            }

            int newOffset = 28;
            foreach (var o in order) { compMap[o] = newOffset; newOffset += recordSize; }

            var specOrder = new List<int>();
            var specMap = new Dictionary<int, int>();
            long specFileLen = _specFs.Length;
            int specOffset = 8;
            while (specOffset < specFileLen)
            {
                _specFs.Seek(specOffset, SeekOrigin.Begin);
                byte del = _specReader.ReadByte();
                _specReader.ReadInt32();
                if (del == 0) specOrder.Add(specOffset);
                specOffset += specRecordSize;
            }
            int newSpecOffset = 8;
            foreach (var o in specOrder) { specMap[o] = newSpecOffset; newSpecOffset += specRecordSize; }

            string tempComp = CurrentFile + ".tmp";
            using (var newFs = new FileStream(tempComp, FileMode.Create))
            using (var bw = new BinaryWriter(newFs))
            {
                bw.Write((byte)'P'); bw.Write((byte)'S');
                bw.Write(len);
                bw.Write(order.Count > 0 ? compMap[order[0]] : -1);
                bw.Write(newOffset);
                byte[] specNameBytes = new byte[16];
                _compFs.Seek(12, SeekOrigin.Begin);
                _compFs.Read(specNameBytes);
                bw.Write(specNameBytes);
                for (int i = 0; i < order.Count; i++)
                {
                    int oldOff = order[i];
                    _compFs.Seek(oldOff, SeekOrigin.Begin);
                    _compReader.ReadByte();
                    int oldSpec = _compReader.ReadInt32();
                    _compReader.ReadInt32();
                    byte type = _compReader.ReadByte();
                    byte[] name = _compReader.ReadBytes(len);
                    int newNext = (i + 1 < order.Count) ? compMap[order[i + 1]] : -1;
                    int newSpec = (oldSpec != -1 && specMap.ContainsKey(oldSpec)) ? specMap[oldSpec] : -1;
                    bw.Write((byte)0); bw.Write(newSpec); bw.Write(newNext); bw.Write(type); bw.Write(name);
                }
            }

            string tempSpec = SpecFile + ".tmp";
            using (var newSfs = new FileStream(tempSpec, FileMode.Create))
            using (var bw = new BinaryWriter(newSfs))
            {
                bw.Write(-1); bw.Write(newSpecOffset);
                foreach (var oldSpecOff in specOrder)
                {
                    _specFs.Seek(oldSpecOff, SeekOrigin.Begin);
                    _specReader.ReadByte();
                    int oldComp = _specReader.ReadInt32();
                    short count = _specReader.ReadInt16();
                    int oldNext = _specReader.ReadInt32();
                    int newComp = compMap.ContainsKey(oldComp) ? compMap[oldComp] : oldComp;
                    int newNext2 = (oldNext != -1 && specMap.ContainsKey(oldNext)) ? specMap[oldNext] : -1;
                    bw.Write((byte)0); bw.Write(newComp); bw.Write(count); bw.Write(newNext2);
                }
            }

            string cf = CurrentFile, sf = SpecFile;
            Close();
            File.Delete(cf); File.Move(tempComp, cf);
            File.Delete(sf); File.Move(tempSpec, sf);
            Open(cf);
        }

        public List<SpecRecord> GetSpecifications(int specHead)
        {
            var result = new List<SpecRecord>();
            int cur = specHead;
            while (cur != -1)
            {
                _specFs.Seek(cur, SeekOrigin.Begin);
                byte del = _specReader.ReadByte();
                int comp = _specReader.ReadInt32();
                short count = _specReader.ReadInt16();
                int next = _specReader.ReadInt32();
                if (del == 0)
                    result.Add(new SpecRecord { Offset = cur, CompOffset = comp, Count = count, Next = next });
                cur = next;
            }
            return result;
        }

        public void AddSpec(int parentOffset, int childOffset)
        {
            _compFs.Seek(parentOffset, SeekOrigin.Begin);
            _compReader.ReadByte();
            int specHead = _compReader.ReadInt32();
            _compReader.ReadInt32();
            byte parentType = _compReader.ReadByte();
            if ((ComponentType)parentType == ComponentType.Деталь)
                throw new Exception("Деталь не может содержать спецификации.");

            if (parentOffset == childOffset)
                throw new Exception("Компонент не может ссылаться сам на себя.");

            int cur = specHead;
            while (cur != -1)
            {
                _specFs.Seek(cur, SeekOrigin.Begin);
                byte del = _specReader.ReadByte();
                int comp = _specReader.ReadInt32();
                short count = _specReader.ReadInt16();
                int next = _specReader.ReadInt32();
                if (comp == childOffset && del == 0)
                {
                    _specFs.Seek(cur + 5, SeekOrigin.Begin);
                    _specWriter.Write((short)(count + 1));
                    _specFs.Flush();
                    return;
                }
                cur = next;
            }

            _specFs.Seek(0, SeekOrigin.End);
            int newPos = (int)_specFs.Position;
            _specWriter.Write((byte)0);
            _specWriter.Write(childOffset);
            _specWriter.Write((short)1);
            _specWriter.Write(specHead);
            int free = (int)_specFs.Position;
            _specFs.Seek(4, SeekOrigin.Begin);
            _specWriter.Write(free);
            _compFs.Seek(parentOffset + 1, SeekOrigin.Begin);
            _compWriter.Write(newPos);
            _compFs.Flush();
            _specFs.Flush();
        }

        public void DeleteSpec(int parentOffset, int childOffset)
        {
            _compFs.Seek(parentOffset + 1, SeekOrigin.Begin);
            int specHead = _compReader.ReadInt32();
            if (specHead == -1) throw new Exception("У компонента нет спецификаций.");

            int cur = specHead, prev = -1;
            while (cur != -1)
            {
                _specFs.Seek(cur, SeekOrigin.Begin);
                byte del = _specReader.ReadByte();
                int comp = _specReader.ReadInt32();
                short count = _specReader.ReadInt16();
                int next = _specReader.ReadInt32();
                if (comp == childOffset && del == 0)
                {
                    if (count > 1)
                    {
                        _specFs.Seek(cur + 5, SeekOrigin.Begin);
                        _specWriter.Write((short)(count - 1));
                    }
                    else
                    {
                        _specFs.Seek(cur, SeekOrigin.Begin);
                        _specWriter.Write((byte)1);
                        if (prev == -1)
                        {
                            _compFs.Seek(parentOffset + 1, SeekOrigin.Begin);
                            _compWriter.Write(next);
                        }
                        else
                        {
                            _specFs.Seek(prev + 1 + 4 + 2, SeekOrigin.Begin);
                            _specWriter.Write(next);
                        }
                    }
                    _specFs.Flush(); _compFs.Flush();
                    return;
                }
                prev = cur; cur = next;
            }
            throw new Exception("Спецификация не найдена.");
        }

        private bool IsComponentReferenced(int componentOffset)
        {
            if (_specFs == null) return false;
            _compFs.Seek(2, SeekOrigin.Begin);
            short len = _compReader.ReadInt16();
            int head = _compReader.ReadInt32();
            while (head != -1)
            {
                _compFs.Seek(head, SeekOrigin.Begin);
                byte del = _compReader.ReadByte();
                int specHead = _compReader.ReadInt32();
                int next = _compReader.ReadInt32();
                _compReader.ReadByte();
                _compReader.ReadChars(len);
                if (del == 0 && specHead != -1)
                {
                    int cur = specHead;
                    while (cur != -1)
                    {
                        _specFs.Seek(cur, SeekOrigin.Begin);
                        byte sd = _specReader.ReadByte();
                        int comp = _specReader.ReadInt32();
                        _specReader.ReadInt16();
                        int sn = _specReader.ReadInt32();
                        if (comp == componentOffset && sd == 0) return true;
                        cur = sn;
                    }
                }
                head = next;
            }
            return false;
        }

        private void DeleteSpecChain(int specHead)
        {
            int cur = specHead;
            while (cur != -1)
            {
                _specFs.Seek(cur, SeekOrigin.Begin);
                _specReader.ReadByte();
                _specReader.ReadInt32();
                _specReader.ReadInt16();
                int next = _specReader.ReadInt32();
                _specFs.Seek(cur, SeekOrigin.Begin);
                _specWriter.Write((byte)1);
                cur = next;
            }
        }

        private void RestoreSpecChain(int specHead)
        {
            int cur = specHead;
            while (cur != -1)
            {
                _specFs.Seek(cur, SeekOrigin.Begin);
                _specReader.ReadByte();
                _specReader.ReadInt32();
                _specReader.ReadInt16();
                int next = _specReader.ReadInt32();
                _specFs.Seek(cur, SeekOrigin.Begin);
                _specWriter.Write((byte)0);
                cur = next;
            }
        }
    }
}
