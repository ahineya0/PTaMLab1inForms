using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiLinkedLists
{
    public partial class ComponentsForm : Form
    {
        private bool _editing = false;
        private int _selectedOffset = -1;

        // Colours for deleted rows
        private static readonly Color DeletedFore = Color.Gray;
        private static readonly Color DeletedBack = Color.FromArgb(245, 245, 245);

        public ComponentsForm()
        {
            InitializeComponent();
            LoadComponents();
        }

        private void LoadComponents()
        {
            listView1.Items.Clear();
            // Pass includeDeleted=true so deleted records are visible
            foreach (var c in FileManager.Instance.GetAllComponents(includeDeleted: true))
            {
                var item = new ListViewItem(c.Name);
                item.SubItems.Add(c.Type.ToString());
                item.SubItems.Add(c.IsDeleted ? "Deleted" : "Active");
                item.Tag = c;

                if (c.IsDeleted)
                {
                    item.ForeColor = DeletedFore;
                    item.BackColor = DeletedBack;
                    // Strikethrough font to make it extra obvious
                    item.Font = new Font(listView1.Font, FontStyle.Strikeout);
                }

                listView1.Items.Add(item);
            }
            UpdateButtonState();
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
            if (c.IsDeleted)
            {
                MessageBox.Show("Cannot edit a deleted record. Restore it first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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
                MessageBox.Show("Enter a name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("Select a type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (c.IsDeleted)
            {
                MessageBox.Show("This record is already deleted.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var r = MessageBox.Show($"Mark component '{c.Name}' as deleted?", "Confirm",
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
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var c = (Component)listView1.SelectedItems[0].Tag;
            if (!c.IsDeleted)
            {
                MessageBox.Show("This record is not deleted.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                FileManager.Instance.RestoreComponent(c.Offset);
                LoadComponents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestoreAll_Click(object sender, EventArgs e)
        {
            var r = MessageBox.Show("Restore all deleted records?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                try
                {
                    FileManager.Instance.RestoreAll();
                    LoadComponents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            UpdateButtonState();
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
            btnRestore.Enabled = !editing;
            btnRestoreAll.Enabled = !editing;
            if (!editing) UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            if (listView1.SelectedItems.Count == 0)
            {
                btnRestore.Enabled = false;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                return;
            }
            var c = (Component)listView1.SelectedItems[0].Tag;
            btnRestore.Enabled = c.IsDeleted;
            btnEdit.Enabled = !c.IsDeleted;
            btnDelete.Enabled = !c.IsDeleted;
        }
    }
}