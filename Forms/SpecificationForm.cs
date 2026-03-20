using MultiLinkedLists.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MultiLinkedLists
{
    public partial class SpecificationForm : Form
    {
        private List<Component> _allComponents;

        public SpecificationForm()
        {
            InitializeComponent();
            LoadTree();
        }

        private void LoadTree()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            _allComponents = FileManager.Instance.GetAllComponents(includeDeleted: true);

            foreach (var c in _allComponents)
            {
                // В верхнем уровне отображаем каждый компонент как корневой узел.
                var node = MakeComponentNode(c, visitedOffsets: new HashSet<int>());
                treeView1.Nodes.Add(node);
            }

            treeView1.ExpandAll();
            treeView1.EndUpdate();
        }

        private TreeNode MakeComponentNode(Component comp, HashSet<int> visitedOffsets)
        {
            string label = comp.IsDeleted ? $"[УДАЛЁН] {comp.Name}" : comp.Name;
            var node = new TreeNode(label) { Tag = comp };
            ApplyDeletedStyle(node, comp.IsDeleted);

            if (comp.SpecHead == -1) return node;

            // Защита от циклических ссылок.
            if (visitedOffsets.Contains(comp.Offset)) return node;
            visitedOffsets.Add(comp.Offset);

            var specs = FileManager.Instance.GetSpecifications(comp.SpecHead, includeDeleted: true);
            foreach (var s in specs)
            {
                var child = _allComponents.Find(x => x.Offset == s.CompOffset);
                if (child == null) continue;

                string childLabel = s.IsDeleted ? $"[УДАЛЁН] {child.Name}  ×{s.Count}" : $"{child.Name}  ×{s.Count}";

                // Рекурсивный подход.
                var childNode = MakeComponentNode(child, new HashSet<int>(visitedOffsets));
                childNode.Text = childLabel;
                childNode.Tag = new object[] { comp, child, s };

                if (s.IsDeleted) ApplyDeletedStyle(childNode, true);

                node.Nodes.Add(childNode);
            }

            visitedOffsets.Remove(comp.Offset);
            return node;
        }

        private static void ApplyDeletedStyle(TreeNode node, bool deleted)
        {
            if (!deleted) return;
            node.ForeColor = Color.Gray;
            node.NodeFont = new Font(SystemFonts.DefaultFont, FontStyle.Strikeout);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search)) { LoadTree(); return; }

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            _allComponents = FileManager.Instance.GetAllComponents(includeDeleted: true);

            foreach (var c in _allComponents)
            {
                if (!c.Name.ToLower().Contains(search)) continue;
                // Отображаем найденный компонент вместе с его полным рекурсивным поддеревом.
                treeView1.Nodes.Add(MakeComponentNode(c, new HashSet<int>()));
            }

            treeView1.ExpandAll();
            treeView1.EndUpdate();
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null) { e.Cancel = true; return; }

            bool isSpecEntry = node.Tag is object[];   // child node = spec link
            bool isDeleted = false;

            if (!isSpecEntry && node.Tag is Component rc)
                isDeleted = rc.IsDeleted;
            else if (isSpecEntry && node.Tag is object[] tags && tags[2] is SpecRecord sr)
                isDeleted = sr.IsDeleted;

            // "Добавить" только на не удаляемый компонент и не деталь
            menuItemAdd.Enabled = !isSpecEntry && node.Tag is Component comp && !comp.IsDeleted && comp.Type != ComponentType.Detail;

            menuItemDelete.Enabled = isSpecEntry && !isDeleted;
            menuItemRestore.Enabled = isSpecEntry && isDeleted;
        }

        private void menuItemAdd_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || node.Tag is not Component parent) return;

            using var dlg = new SelectComponentDialog(parent);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileManager.Instance.AddSpec(parent.Offset, dlg.SelectedComponent.Offset);
                    LoadTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuItemDelete_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || node.Tag is not object[] tags) return;
            var parent = (Component)tags[0];
            var child = (Component)tags[1];

            var r = MessageBox.Show(
                $"Убрать '{child.Name}' из спецификации '{parent.Name}'?", "Подтвердить", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                try
                {
                    FileManager.Instance.DeleteSpec(parent.Offset, child.Offset);
                    LoadTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuItemRestore_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || node.Tag is not object[] tags) return;
            var parent = (Component)tags[0];
            var child = (Component)tags[1];

            try
            {
                FileManager.Instance.RestoreSpec(parent.Offset, child.Offset);
                LoadTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeView1.SelectedNode = e.Node;
        }
    }
}