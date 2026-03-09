using System;
using System.Windows.Forms;

namespace MultiLinkedLists
{
    public partial class ComponentsForm : Form
    {
        private bool _editing = false;
        private int _selectedOffset = -1;

        public ComponentsForm()
        {
            InitializeComponent();
            LoadComponents();
        }

        private void LoadComponents()
        {
            listView1.Items.Clear();
            var components = FileManager.Instance.GetAllComponents();
            foreach (var c in components)
            {
                var item = new ListViewItem(c.Name);
                item.SubItems.Add(c.Type.ToString());
                item.Tag = c;
                listView1.Items.Add(item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _editing = false;
            _selectedOffset = -1;
            txtName.Clear();
            cmbType.SelectedIndex = 0;
            SetEditState(true);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var c = (Component)listView1.SelectedItems[0].Tag;
            _editing = true;
            _selectedOffset = c.Offset;
            txtName.Text = c.Name;
            cmbType.SelectedItem = c.Type.ToString();
            SetEditState(true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите тип.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var type = (ComponentType)Enum.Parse(typeof(ComponentType), cmbType.SelectedItem.ToString());
            try
            {
                if (_editing)
                    FileManager.Instance.UpdateComponent(_selectedOffset, txtName.Text.Trim(), type);
                else
                    FileManager.Instance.AddComponent(txtName.Text.Trim(), type);

                LoadComponents();
                SetEditState(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditState(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var c = (Component)listView1.SelectedItems[0].Tag;
            var r = MessageBox.Show($"Удалить компонент '{c.Name}'?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                try
                {
                    FileManager.Instance.DeleteComponent(c.Offset);
                    LoadComponents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var c = (Component)listView1.SelectedItems[0].Tag;
                txtName.Text = c.Name;
                cmbType.SelectedItem = c.Type.ToString();
                _selectedOffset = c.Offset;
            }
        }

        private void SetEditState(bool editing)
        {
            btnSave.Enabled = editing;
            btnCancel.Enabled = editing;
            txtName.Enabled = editing;
            cmbType.Enabled = editing;
            btnAdd.Enabled = !editing;
            btnEdit.Enabled = !editing;
            btnDelete.Enabled = !editing;
        }
    }
}
