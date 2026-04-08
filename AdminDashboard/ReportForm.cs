using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RestaurantDesktopApp
{
    public partial class ReportForm : Form
    {
        MySqlConnection con = new MySqlConnection(
            "server=localhost;user=root;password=;database=RestaurantDB");

        private Button btnExport;

        public ReportForm()
        {
            InitializeComponent();

            SetupUI();
            ApplyStyles();

            lblTotalSales.Text = "Click Refresh";
            dgvReport.DataSource = null;
        }

        // ================= UI SETUP =================
        private void SetupUI()
        {
            // Export Button
            btnExport = new Button();
            btnExport.Text = "Export CSV";
            btnExport.Size = new Size(140, 35);
            btnExport.Location = new Point(20, 20);
            btnExport.Click += (s, e) =>
                UIHelper.ExportToCSV(dgvReport, "Sales_" + DateTime.Now.ToString("yyyyMMdd"));

            this.Controls.Add(btnExport);

            // Move Refresh button beside export
            btnRefresh.Location = new Point(180, 20);

            // Move DataGrid
            dgvReport.Location = new Point(20, 80);
            dgvReport.Size = new Size(900, 400);

            StyleGrid();
        }

        // ================= STYLE =================
        private void ApplyStyles()
        {
            UIHelper.ApplyModernButton(btnExport, UIHelper.SuccessColor);
            UIHelper.ApplyModernButton(btnRefresh, UIHelper.AccentColor);

            UIHelper.SetRoundedRegion(cardRevenue, 15);
            UIHelper.SetRoundedRegion(cardOrders, 15);
            UIHelper.SetRoundedRegion(cardTables, 15);
            UIHelper.SetRoundedRegion(cardPending, 15);
        }

        private void StyleGrid()
        {
            dgvReport.BorderStyle = BorderStyle.None;
            dgvReport.BackgroundColor = Color.White;

            dgvReport.EnableHeadersVisualStyles = false;
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(59, 130, 246);
            dgvReport.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvReport.RowTemplate.Height = 35;
        }

        // ================= LOAD STATS =================
        private void LoadStats()
        {
            try
            {
                using (con)
                {
                    con.Open();

                    // Revenue
                    MySqlCommand revCmd = new MySqlCommand(
                        "SELECT SUM(TotalAmount) FROM Orders WHERE Status='Paid'", con);
                    object rev = revCmd.ExecuteScalar();

                    lblRevenueVal.Text =
                        (rev != DBNull.Value && rev != null)
                        ? $"{UIHelper.GetCurrencySymbol()} {Convert.ToDecimal(rev):N2}"
                        : $"{UIHelper.GetCurrencySymbol()} 0.00";

                    // Orders
                    MySqlCommand ordCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Orders", con);
                    lblOrdersVal.Text = ordCmd.ExecuteScalar()?.ToString() ?? "0";

                    // Active Tables
                    MySqlCommand tblCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Tables WHERE Status='Occupied'", con);
                    lblTablesVal.Text = tblCmd.ExecuteScalar()?.ToString() ?? "0";

                    // Pending Orders
                    MySqlCommand pendCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE Status='Pending'", con);
                    lblPendingVal.Text = pendCmd.ExecuteScalar()?.ToString() ?? "0";
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowToast("Stats error: " + ex.Message, true);
            }
        }

        // ================= REPORT =================
        private void LoadDailyReport()
        {
            try
            {
                string query = @"
                    SELECT 
                        DATE(OrderDate) as Date,
                        COUNT(OrderID) as TotalOrders,
                        SUM(TotalAmount) as TotalSales
                    FROM Orders
                    WHERE Status='Paid'
                    AND DATE(OrderDate) = CURDATE()
                    GROUP BY DATE(OrderDate)";

                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvReport.DataSource = dt;

                // Format currency column
                if (dgvReport.Columns.Contains("TotalSales"))
                {
                    dgvReport.Columns["TotalSales"].DefaultCellStyle.Format =
                        UIHelper.GetCurrencySymbol() + " #,##0.00";
                }

                // Total label
                if (dt.Rows.Count > 0 && dt.Rows[0]["TotalSales"] != DBNull.Value)
                {
                    lblTotalSales.Text =
                        UIHelper.GetCurrencySymbol() + " " +
                        Convert.ToDecimal(dt.Rows[0]["TotalSales"]).ToString("N2");
                }
                else
                {
                    lblTotalSales.Text = UIHelper.GetCurrencySymbol() + " 0.00";
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowToast("Report failed: " + ex.Message, true);
            }
        }

        // ================= EVENTS =================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStats();
            LoadDailyReport();
        }

        private void dgvReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // ================= VISUAL EFFECT =================
        private void cardRevenue_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen p = new Pen(Color.FromArgb(50, UIHelper.SuccessColor), 2);

            int[] values = { 20, 40, 35, 60, 50, 80, 70 };
            int step = cardRevenue.Width / (values.Length + 1);

            for (int i = 0; i < values.Length; i++)
            {
                g.DrawLine(p,
                    step * (i + 1),
                    cardRevenue.Height - 10,
                    step * (i + 1),
                    cardRevenue.Height - values[i]);
            }
        }
    }
}