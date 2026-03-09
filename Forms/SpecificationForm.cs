using MultiLinkedLists.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

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
            var components = FileManager.Instance.GetAllComponents(includeDeleted: true);

            foreach (var c in components)
            {
                string parentLabel = c.IsDeleted ? $"[УДАЛЁН] {c.Name}" : c.Name;
                var node = new TreeNode(parentLabel) { Tag = c };

                if (c.IsDeleted)
                {
                    node.ForeColor = Color.Gray;
                    node.NodeFont = new Font(treeView1.Font, FontStyle.Strikeout);
                }

                if (c.SpecHead != -1)
                {
                    var specs = FileManager.Instance.GetSpecifications(c.SpecHead, includeDeleted: true);
                    foreach (var s in specs)
                    {
                        var child = components.Find(x => x.Offset == s.CompOffset);
                        if (child == null) continue;

                        string childLabel = s.IsDeleted
                            ? $"[УДАЛЁН] {child.Name}  ×{s.Count}"
                            : $"{child.Name}  ×{s.Count}";

                        var childNode = new TreeNode(childLabel)
                        {
                            Tag = new object[] { c, child, s }
                        };

                        if (s.IsDeleted)
                        {
                            childNode.ForeColor = Color.Gray;
                            childNode.NodeFont = new Font(treeView1.Font, FontStyle.Strikeout);
                        }

                        node.Nodes.Add(childNode);
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
            foreach (var c in FileManager.Instance.GetAllComponents(includeDeleted: true))
            {
                if (c.Name.ToLower().Contains(search))
                {
                    string label = c.IsDeleted ? $"[УДАЛЁН] {c.Name}" : c.Name;
                    var node = new TreeNode(label) { Tag = c };
                    if (c.IsDeleted) node.ForeColor = Color.Gray;
                    treeView1.Nodes.Add(node);
                }
            }
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null) { e.Cancel = true; return; }

            bool isRoot = node.Parent == null;
            bool isDeleted = false;

            if (isRoot && node.Tag is Component rc)
                isDeleted = rc.IsDeleted;
            else if (!isRoot && node.Tag is object[] tags && tags[2] is SpecRecord sr)
                isDeleted = sr.IsDeleted;

            menuItemAdd.Enabled = isRoot
                                      && node.Tag is Component comp
                                      && !comp.IsDeleted
                                      && comp.Type != ComponentType.Detail;
            menuItemDelete.Enabled = !isRoot && !isDeleted;
            menuItemRestore.Enabled = !isRoot && isDeleted;
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
                    MessageBox.Show(ex.Message, "Ошибка.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                $"Удалить '{child.Name}' из спецификации '{parent.Name}'?",
                "Подтвердить", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                try
                {
                    FileManager.Instance.DeleteSpec(parent.Offset, child.Offset);
                    LoadTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Ошибка.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeView1.SelectedNode = e.Node;
        }
    }
}