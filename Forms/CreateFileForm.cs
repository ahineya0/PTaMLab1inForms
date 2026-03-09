using System;
using System.IO;
using System.Windows.Forms;

namespace MultiLinkedLists.Forms
{
    public class CreateFileForm : Form
    {
        private TextBox txtFile, txtSpecFile, txtMaxLen;
        private Button btnBrowse, btnBrowseSpec, btnCreate, btnCancel;
        private Label lblFile, lblSpec, lblLen;
        private NumericUpDown numLen;

        public CreateFileForm()
        {
            Text = "Создать новый файл";
            Size = new System.Drawing.Size(420, 230);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            lblFile = new Label { Text = "Файл компонентов:", Location = new System.Drawing.Point(10, 15), AutoSize = true };
            txtFile = new TextBox { Location = new System.Drawing.Point(10, 35), Size = new System.Drawing.Size(290, 23) };
            btnBrowse = new Button { Text = "...", Location = new System.Drawing.Point(308, 34), Size = new System.Drawing.Size(30, 25) };

            lblSpec = new Label { Text = "Файл спецификаций:", Location = new System.Drawing.Point(10, 70), AutoSize = true };
            txtSpecFile = new TextBox { Location = new System.Drawing.Point(10, 90), Size = new System.Drawing.Size(290, 23) };
            btnBrowseSpec = new Button { Text = "...", Location = new System.Drawing.Point(308, 89), Size = new System.Drawing.Size(30, 25) };

            lblLen = new Label { Text = "Макс. длина имени:", Location = new System.Drawing.Point(10, 125), AutoSize = true };
            numLen = new NumericUpDown { Location = new System.Drawing.Point(10, 145), Size = new System.Drawing.Size(80, 23), Minimum = 8, Maximum = 64, Value = 20 };

            btnCreate = new Button { Text = "Создать", Location = new System.Drawing.Point(240, 155), Size = new System.Drawing.Size(80, 28) };
            btnCancel = new Button { Text = "Отмена", Location = new System.Drawing.Point(328, 155), Size = new System.Drawing.Size(70, 28), DialogResult = DialogResult.Cancel };

            btnBrowse.Click += (s, e) =>
            {
                using var sfd = new SaveFileDialog { Filter = "Файлы компонентов (*.prd)|*.prd", Title = "Файл компонентов" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txtFile.Text = sfd.FileName;
                    if (string.IsNullOrEmpty(txtSpecFile.Text))
                        txtSpecFile.Text = Path.ChangeExtension(sfd.FileName, ".prs");
                }
            };

            btnBrowseSpec.Click += (s, e) =>
            {
                using var sfd = new SaveFileDialog { Filter = "Файлы спецификаций (*.prs)|*.prs", Title = "Файл спецификаций" };
                if (sfd.ShowDialog() == DialogResult.OK) txtSpecFile.Text = sfd.FileName;
            };

            btnCreate.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtFile.Text))
                {
                    MessageBox.Show("Укажите файл компонентов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    string specFile = string.IsNullOrWhiteSpace(txtSpecFile.Text)
                        ? Path.ChangeExtension(txtFile.Text, ".prs")
                        : txtSpecFile.Text;

                    bool ok = FileManager.CreateNew(txtFile.Text, (short)numLen.Value, specFile);
                    if (ok)
                    {
                        MessageBox.Show("Файл успешно создан.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Controls.AddRange(new Control[] {
                lblFile, txtFile, btnBrowse,
                lblSpec, txtSpecFile, btnBrowseSpec,
                lblLen, numLen,
                btnCreate, btnCancel
            });

            CancelButton = btnCancel;
        }
    }
}
