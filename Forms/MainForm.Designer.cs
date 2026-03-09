using System.Windows.Forms;

namespace MultiLinkedLists
{
    partial class MainForm
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
            menuOpen = new ToolStripMenuItem();
            menuCreate = new ToolStripMenuItem();
            menuComponents = new ToolStripMenuItem();
            menuSpecification = new ToolStripMenuItem();
            menuTruncate = new ToolStripMenuItem();

            menuStrip1.SuspendLayout();
            SuspendLayout();

            menuStrip1.Items.AddRange(new ToolStripItem[] {
                menuOpen, menuComponents, menuSpecification, menuTruncate
            });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Size = new System.Drawing.Size(500, 24);
            menuStrip1.TabStop = false;

            menuOpen.Text = "Открыть";
            menuOpen.DropDownItems.Add(menuCreate);
            menuOpen.Click += menuOpen_Click;

            menuCreate.Text = "Создать новый файл...";
            menuCreate.Click += menuCreate_Click;

            menuComponents.Text = "Компоненты";
            menuComponents.Click += menuComponents_Click;

            menuSpecification.Text = "Спецификация";
            menuSpecification.Click += menuSpecification_Click;

            menuTruncate.Text = "Очистить корзину";
            menuTruncate.Click += menuTruncate_Click;

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(500, 200);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "Организация многосвязных структур";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuOpen;
        private ToolStripMenuItem menuCreate;
        private ToolStripMenuItem menuComponents;
        private ToolStripMenuItem menuSpecification;
        private ToolStripMenuItem menuTruncate;
    }
}