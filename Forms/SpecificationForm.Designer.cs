using System.Drawing;
using System.Windows.Forms;

namespace MultiLinkedLists
{
    partial class SpecificationForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            txtSearch = new TextBox();
            btnFind = new Button();
            treeView1 = new TreeView();
            contextMenu = new ContextMenuStrip(components);
            menuItemAdd = new ToolStripMenuItem();
            menuItemDelete = new ToolStripMenuItem();
            menuItemRestore = new ToolStripMenuItem();

            contextMenu.SuspendLayout();
            SuspendLayout();

            txtSearch.Location = new Point(8, 8);
            txtSearch.Size = new Size(280, 23);
            txtSearch.TabIndex = 0;
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnFind_Click(s, e); };

            btnFind.Text = "Найти";
            btnFind.Location = new Point(296, 7);
            btnFind.Size = new Size(80, 25);
            btnFind.TabIndex = 1;
            btnFind.Click += btnFind_Click;

            treeView1.Location = new Point(8, 40);
            treeView1.Size = new Size(400, 360);
            treeView1.TabIndex = 2;
            treeView1.ContextMenuStrip = contextMenu;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;

            menuItemAdd.Text = "Добавить";
            menuItemDelete.Text = "Удалить";
            menuItemRestore.Text = "Восстановить";

            menuItemAdd.Click += menuItemAdd_Click;
            menuItemDelete.Click += menuItemDelete_Click;
            menuItemRestore.Click += menuItemRestore_Click;

            contextMenu.Items.AddRange(new ToolStripItem[] {
                menuItemAdd, menuItemDelete, menuItemRestore
            });
            contextMenu.Opening += contextMenu_Opening;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 412);
            Controls.AddRange(new Control[] { txtSearch, btnFind, treeView1 });
            Text = "Спецификация";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox txtSearch;
        private Button btnFind;
        private TreeView treeView1;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem menuItemAdd;
        private ToolStripMenuItem menuItemDelete;
        private ToolStripMenuItem menuItemRestore;
    }
}