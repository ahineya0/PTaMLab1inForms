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
            contextMenuStrip1 = new ContextMenuStrip(components);
            menuAdd = new ToolStripMenuItem();
            menuEdit = new ToolStripMenuItem();
            menuDelete = new ToolStripMenuItem();

            contextMenuStrip1.SuspendLayout();
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
            treeView1.Size = new Size(368, 330);
            treeView1.TabIndex = 2;
            treeView1.ContextMenuStrip = contextMenuStrip1;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;

            menuAdd.Text = "Добавить";
            menuEdit.Text = "Изменить";
            menuDelete.Text = "Удалить";
            menuAdd.Click += menuAdd_Click;
            menuEdit.Click += menuEdit_Click;
            menuDelete.Click += menuDelete_Click;
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { menuAdd, menuEdit, menuDelete });
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 380);
            Controls.AddRange(new Control[] { txtSearch, btnFind, treeView1 });
            Text = "Спецификация";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox txtSearch;
        private Button btnFind;
        private TreeView treeView1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem menuAdd;
        private ToolStripMenuItem menuEdit;
        private ToolStripMenuItem menuDelete;
    }
}
