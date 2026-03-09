using System;
using System.Windows.Forms;

namespace MultiLinkedLists.Forms
{
    public class SelectComponentDialog : Form
    {
        public Component SelectedComponent { get; private set; }
        private ListBox listBox;
        private Button btnOk, btnCancel;
        private readonly Component _parent;

        public SelectComponentDialog(Component parent)
        {
            _parent = parent;
            InitUI();
            LoadComponents();
        }

        private void InitUI()
        {
            Text = "Выбор компонента";
            Size = new System.Drawing.Size(320, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            listBox = new ListBox { Location = new System.Drawing.Point(8, 8), Size = new System.Drawing.Size(288, 280) };
            btnOk = new Button { Text = "OK", Location = new System.Drawing.Point(8, 296), Size = new System.Drawing.Size(80, 25), DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Отмена", Location = new System.Drawing.Point(96, 296), Size = new System.Drawing.Size(80, 25), DialogResult = DialogResult.Cancel };
            btnOk.Click += (s, e) =>
            {
                if (listBox.SelectedItem is Component c) { SelectedComponent = c; Close(); }
                else MessageBox.Show("Выберите компонент.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Controls.AddRange(new Control[] { listBox, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void LoadComponents()
        {
            var comps = FileManager.Instance.GetAllComponents();
            listBox.DisplayMember = "Name";
            foreach (var c in comps)
            {
                if (c.Offset != _parent.Offset)
                    listBox.Items.Add(c);
            }
        }
    }
}
