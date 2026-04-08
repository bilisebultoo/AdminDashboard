using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RestaurantDesktopApp
{
    public partial class StaffForm : Form
    {
        private MySqlConnection con = new MySqlConnection(
            "server=localhost;user=root;password=;database=RestaurantDB");

        private TextBox txtSearch;

        public StaffForm()
        {
            InitializeComponent();
        }

        private void StaffForm_Load(object sender, EventArgs e)
        {
            SetupUI();
            ApplyStyles();
            LoadStaff();
        }

        // ================= UI =================
        private void SetupUI()
        {
            // Search box
            txtSearch = new TextBox();
            txtSearch.Size = new Size(220, 30);
            txtSearch.Location = new Point(20, 20);
            txtSearch.PlaceholderText = "🔍 Search staff...";
            txtSearch.TextChanged += (s, e) => FilterStaff();
            this.Controls.Add(txtSearch);

            // Move grid
            dgvStaff.Location = new Point(20, 60);
            dgvStaff.Size = new Size(500, 350);

            // Grid click
            dgvStaff.CellClick += dgvStaff_CellClick;

            StyleGrid();
        }

        private void ApplyStyles()
        {
            UIHelper.ApplyModernButton(btnAdd, UIHelper.SuccessColor);
            UIHelper.ApplyModernButton(btnDelete, UIHelper.DangerColor);
            UIHelper.ApplyModernButton(btnUpdate, UIHelper.AccentColor);
        }

        private void StyleGrid()
        {
            dgvStaff.BorderStyle = BorderStyle.None;
            dgvStaff.BackgroundColor = Color.White;

            dgvStaff.EnableHeadersVisualStyles = false;
            dgvStaff.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvStaff.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStaff.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvStaff.DefaultCellStyle.SelectionBackColor = Color.FromArgb(59, 130, 246);
            dgvStaff.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvStaff.RowTemplate.Height = 35;
        }

        // ================= LOAD =================
        private void LoadStaff()
        {
            try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT Username, Role FROM Users", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvStaff.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowToast("Error loading staff: " + ex.Message, true);
            }
        }

        // ================= FILTER =================
        private void FilterStaff()
        {
            if (dgvStaff.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter =
                    $"Username LIKE '%{txtSearch.Text}%' OR Role LIKE '%{txtSearch.Text}%'";
            }
        }

        // ================= ADD =================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                UIHelper.ShowToast("Fill all fields.", true);
                return;
            }

            try
            {
                using (con)
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO Users (Username, Password, Role) VALUES (@u,@p,@r)", con);

                    cmd.Parameters.AddWithValue("@u", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@p", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@r", cmbRole.Text);

                    cmd.ExecuteNonQuery();
                }

                UIHelper.ShowToast("Staff added");
                LoadStaff();
                ClearFields();
            }
            catch (Exception ex)
            {
                UIHelper.ShowToast("Error: " + ex.Message, true);
            }
        }

        // ================= DELETE =================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count == 0) return;

            string user = dgvStaff.SelectedRows[0].Cells["Username"].Value.ToString();

            if (user == "admin")
            {
                UIHelper.ShowToast("Cannot delete admin.", true);
                return;
            }

            if (MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo)
                != DialogResult.Yes) return;

            try
            {
                using (con)
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM Users WHERE Username=@u", con);
                    cmd.Parameters.AddWithValue("@u", user);

                    cmd.ExecuteNonQuery();
                }

                UIHelper.ShowToast("Deleted");
                LoadStaff();
            }
            catch (Exception ex)
            {
                UIHelper.ShowToast("Error: " + ex.Message, true);
            }
        }

        // ================= UPDATE =================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count == 0)
            {
                UIHelper.ShowToast("Select a user first.", true);
                return;
            }

            string user = dgvStaff.SelectedRows[0].Cells["Username"].Value.ToString();

            if (user == "admin")
            {
                UIHelper.ShowToast("Cannot modify admin.", true);
                return;
            }

            try
            {
                using (con)
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(
                        "UPDATE Users SET Password=@p, Role=@r WHERE Username=@u", con);

                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@r", cmbRole.Text);

                    cmd.ExecuteNonQuery();
                }

                UIHelper.ShowToast("Updated");
                LoadStaff();
                ClearFields();
            }
            catch (Exception ex)
            {
                UIHelper.ShowToast("Error: " + ex.Message, true);
            }
        }

        // ================= SELECT =================
        private void dgvStaff_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvStaff.Rows[e.RowIndex];

            txtUsername.Text = row.Cells["Username"].Value?.ToString();
            cmbRole.Text = row.Cells["Role"].Value?.ToString();

            txtPassword.Clear(); // security
        }

        // ================= UTIL =================
        private void ClearFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = 0;
        }
    }
}