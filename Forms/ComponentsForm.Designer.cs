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
            listView1 = new ListView();
            colName = new ColumnHeader();
            colType = new ColumnHeader();
            lblName = new Label();
            txtName = new TextBox();
            lblType = new Label();
            cmbType = new ComboBox();
            panelBottom = new Panel();

            menuStrip1.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();

            // menuStrip
            menuStrip1.Items.AddRange(new ToolStripItem[] { btnAdd, btnEdit, btnCancel, btnSave, btnDelete });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Size = new Size(560, 24);
            menuStrip1.TabStop = false;

            btnAdd.Text = "Добавить";
            btnEdit.Text = "Изменить";
            btnCancel.Text = "Отменить";
            btnSave.Text = "Сохранить";
            btnDelete.Text = "Удалить";

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnCancel.Click += btnCancel_Click;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;

            btnSave.Enabled = false;
            btnCancel.Enabled = false;

            // listView1
            listView1.Columns.AddRange(new ColumnHeader[] { colName, colType });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(0, 24);
            listView1.Size = new Size(560, 260);
            listView1.View = View.Details;
            listView1.MultiSelect = false;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            colName.Text = "Наименование";
            colName.Width = 300;
            colType.Text = "Тип";
            colType.Width = 257;

            // panelBottom
            panelBottom.Location = new Point(0, 284);
            panelBottom.Size = new Size(560, 36);
            panelBottom.BorderStyle = BorderStyle.FixedSingle;

            lblName.Text = "Наименование";
            lblName.Location = new Point(4, 10);
            lblName.AutoSize = true;

            txtName.Location = new Point(100, 7);
            txtName.Size = new Size(250, 23);
            txtName.Enabled = false;

            lblType.Text = "Тип";
            lblType.Location = new Point(360, 10);
            lblType.AutoSize = true;

            cmbType.Location = new Point(385, 7);
            cmbType.Size = new Size(120, 23);
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.AddRange(new object[] { "Изделие", "Узел", "Деталь" });
            cmbType.Enabled = false;

            panelBottom.Controls.AddRange(new Control[] { lblName, txtName, lblType, cmbType });

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 322);
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
        private ListView listView1;
        private ColumnHeader colName;
        private ColumnHeader colType;
        private Label lblName;
        private TextBox txtName;
        private Label lblType;
        private ComboBox cmbType;
        private Panel panelBottom;
    }
}
