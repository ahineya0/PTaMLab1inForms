using System.Drawing;
using System.Windows.Forms;

namespace MultiLinkedLists
{
    partial class ComponentsForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            btnAdd = new ToolStripMenuItem();
            btnEdit = new ToolStripMenuItem();
            btnCancel = new ToolStripMenuItem();
            btnSave = new ToolStripMenuItem();
            btnDelete = new ToolStripMenuItem();
            btnRestore = new ToolStripMenuItem();
            btnRestoreAll = new ToolStripMenuItem();
            listView1 = new ListView();
            colName = new ColumnHeader();
            colType = new ColumnHeader();
            colStatus = new ColumnHeader();
            lblName = new Label();
            txtName = new TextBox();
            lblType = new Label();
            cmbType = new ComboBox();
            panelBottom = new Panel();

            menuStrip1.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();

            menuStrip1.Items.AddRange(new ToolStripItem[] {
                btnAdd, btnEdit, btnCancel, btnSave, btnDelete, btnRestore, btnRestoreAll
            });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Size = new Size(620, 24);
            menuStrip1.TabStop = false;

            btnAdd.Text = "Добавить";
            btnEdit.Text = "Изменить";
            btnCancel.Text = "Отмена";
            btnSave.Text = "Сохранить";
            btnDelete.Text = "Удалить";
            btnRestore.Text = "Восстановить";
            btnRestoreAll.Text = "Восстановить всё";

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnCancel.Click += btnCancel_Click;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
            btnRestore.Click += btnRestore_Click;
            btnRestoreAll.Click += btnRestoreAll_Click;

            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnRestore.Enabled = false;

            listView1.Columns.AddRange(new ColumnHeader[] { colName, colType, colStatus });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(0, 24);
            listView1.Size = new Size(620, 260);
            listView1.View = View.Details;
            listView1.MultiSelect = false;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            colName.Text = "Имя";
            colName.Width = 280;
            colType.Text = "Тип";
            colType.Width = 160;
            colStatus.Text = "Статус";
            colStatus.Width = 177;

            panelBottom.Location = new Point(0, 284);
            panelBottom.Size = new Size(620, 36);
            panelBottom.BorderStyle = BorderStyle.FixedSingle;

            lblName.Text = "Имя";
            lblName.Location = new Point(4, 10);
            lblName.AutoSize = true;

            txtName.Location = new Point(60, 7);
            txtName.Size = new Size(290, 23);
            txtName.Enabled = false;

            lblType.Text = "Тип";
            lblType.Location = new Point(362, 10);
            lblType.AutoSize = true;

            cmbType.Location = new Point(395, 7);
            cmbType.Size = new Size(110, 23);
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.AddRange(new object[] { "Product", "Unit", "Detail" });
            cmbType.Enabled = false;

            panelBottom.Controls.AddRange(new Control[] { lblName, txtName, lblType, cmbType });

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, 322);
            Controls.AddRange(new Control[] { menuStrip1, listView1, panelBottom });
            MainMenuStrip = menuStrip1;
            Text = "Список компонентов";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem btnAdd;
        private ToolStripMenuItem btnEdit;
        private ToolStripMenuItem btnCancel;
        private ToolStripMenuItem btnSave;
        private ToolStripMenuItem btnDelete;
        private ToolStripMenuItem btnRestore;
        private ToolStripMenuItem btnRestoreAll;
        private ListView listView1;
        private ColumnHeader colName;
        private ColumnHeader colType;
        private ColumnHeader colStatus;
        private Label lblName;
        private TextBox txtName;
        private Label lblType;
        private ComboBox cmbType;
        private Panel panelBottom;
    }
}