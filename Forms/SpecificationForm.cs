using System;
using System.Windows.Forms;
using MultiLinkedLists.Forms;

namespace MultiLinkedLists
{
    public partial class SpecificationForm : Form
    {
        public SpecificationForm()
        {
            InitializeComponent();
            LoadTree();
        }

        private void LoadTree()
        {
            treeView1.Nodes.Clear();
            var components = FileManager.Instance.GetAllComponents();
            foreach (var c in components)
            {
                var node = new TreeNode(c.Name) { Tag = c };
                if (c.SpecHead != -1)
                {
                    var specs = FileManager.Instance.GetSpecifications(c.SpecHead);
                    foreach (var s in specs)
                    {
                        var allComps = FileManager.Instance.GetAllComponents();
                        var child = allComps.Find(x => x.Offset == s.CompOffset);
                        if (child != null)
                        {
                            var childNode = new TreeNode($"{child.Name}") { Tag = new object[] { c, child, s } };
                            node.Nodes.Add(childNode);
                        }
                    }
                }
                treeView1.Nodes.Add(node);
            }
            treeView1.ExpandAll();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search)) { LoadTree(); return; }

            treeView1.Nodes.Clear();
            var components = FileManager.Instance.GetAllComponents();
            foreach (var c in components)
            {
                if (c.Name.ToLower().Contains(search))
                {
                    var node = new TreeNode(c.Name) { Tag = c };
                    treeView1.Nodes.Add(node);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null) { e.Cancel = true; return; }

            bool isRoot = node.Parent == null;
            menuAdd.Enabled = isRoot && node.Tag is Component comp && comp.Type != ComponentType.Деталь;
            menuEdit.Enabled = !isRoot;
            menuDelete.Enabled = !isRoot;
        }

        private void menuAdd_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || !(node.Tag is Component parent)) return;

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

        private void menuEdit_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || !(node.Tag is object[] tags)) return;
            var parent = (Component)tags[0];
            var child = (Component)tags[1];
            var spec = (SpecRecord)tags[2];

            MessageBox.Show($"Компонент: {parent.Name}\nДеталь: {child.Name}\nКратность: {spec.Count}",
                "Информация о спецификации", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || !(node.Tag is object[] tags)) return;
            var parent = (Component)tags[0];
            var child = (Component)tags[1];

            var r = MessageBox.Show($"Удалить '{child.Name}' из спецификации '{parent.Name}'?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeView1.SelectedNode = e.Node;
        }
    }
}
