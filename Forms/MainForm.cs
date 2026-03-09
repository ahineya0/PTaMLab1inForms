using System;
using System.Windows.Forms;
using MultiLinkedLists.Forms;

namespace MultiLinkedLists
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Component files (*.prd)|*.prd|All files (*.*)|*.*",
                Title = "Открыть файл компонентов"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileManager.Instance.Open(ofd.FileName);
                    MessageBox.Show(
                        $"Файл компонентов: {FileManager.Instance.CurrentFile}\n" +
                        $"Файл спецификации: {FileManager.Instance.SpecFile}",
                        "Файлы открыты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuCreate_Click(object sender, EventArgs e)
        {
            new CreateFileForm().ShowDialog();
        }

        private void menuComponents_Click(object sender, EventArgs e)
        {
            if (!FileManager.Instance.IsOpen)
            {
                MessageBox.Show("Пожалуйста, сначала откройте файл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            new ComponentsForm().ShowDialog();
        }

        private void menuSpecification_Click(object sender, EventArgs e)
        {
            if (!FileManager.Instance.IsOpen)
            {
                MessageBox.Show("Пожалуйста, сначала откройте файл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            new SpecificationForm().ShowDialog();
        }

        private void menuTruncate_Click(object sender, EventArgs e)
        {
            if (!FileManager.Instance.IsOpen)
            {
                MessageBox.Show("Пожалуйста, сначала откройте файл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = MessageBox.Show(
                "Окончательно удалить записи?\nЭто действие не отменить.",
                "Подтвердить", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    FileManager.Instance.Truncate();
                    MessageBox.Show("Физическое удаление завершено.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}