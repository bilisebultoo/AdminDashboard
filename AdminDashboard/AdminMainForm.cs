using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RestaurantDesktopApp
{
    public partial class AdminMainForm : Form
    {
        private MySqlConnection con = new MySqlConnection("server=localhost;user=root;password=;database=RestaurantDB");

        public AdminMainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Opacity = 0;
        }

        private void AdminMainForm_Load(object sender, EventArgs e)
        {
            fadeTimer.Start();
            ApplyStyles();
            UIHelper.ApplySidebarStyle(sidebarPanel);
            ShowDashboardOverview();
        }

        // ================= DASHBOARD =================
        private void ShowDashboardOverview()
        {
            contentPanel.Controls.Clear();
            lblPanelTitle.Text = "Dashboard Overview";

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.Padding = new Padding(40, 30, 40, 30);
            flow.AutoScroll = true;
            flow.BackColor = Color.FromArgb(245, 247, 250);
            contentPanel.Controls.Add(flow);

            // STAT CARDS
            AddStatCard(flow, "Total Revenue", "12,450 ETB", Color.FromArgb(16, 185, 129));
            AddStatCard(flow, "Daily Orders", "48", Color.FromArgb(59, 130, 246));
            AddStatCard(flow, "Available Tables", "12/20", Color.FromArgb(245, 158, 11));
            AddStatCard(flow, "Active Staff", "6", Color.FromArgb(139, 92, 246));

            // ACTIVITY PANEL
            Panel activityPanel = new Panel();
            activityPanel.Width = flow.ClientSize.Width - 80;
            activityPanel.Height = 420;
            activityPanel.BackColor = Color.White;
            activityPanel.Margin = new Padding(0, 30, 0, 0);

            UIHelper.SetRoundedRegion(activityPanel, 20);

            activityPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 220)), 0, 0, activityPanel.Width - 1, activityPanel.Height - 1);
            };

            flow.Controls.Add(activityPanel);

            Label lblAct = new Label();
            lblAct.Text = "Recent Activity";
            lblAct.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblAct.ForeColor = Color.FromArgb(30, 41, 59);
            lblAct.Location = new Point(25, 20);
            lblAct.AutoSize = true;
            activityPanel.Controls.Add(lblAct);

            ListBox lstLog = new ListBox();
            lstLog.BorderStyle = BorderStyle.None;
            lstLog.Font = new Font("Segoe UI", 11);
            lstLog.ForeColor = Color.FromArgb(71, 85, 105);
            lstLog.Location = new Point(25, 70);
            lstLog.Size = new Size(activityPanel.Width - 50, 300);
            lstLog.ItemHeight = 35;

            lstLog.Items.Add("✔ Order #1023 completed - 210 ETB");
            lstLog.Items.Add("📦 New stock: Chicken (20kg)");
            lstLog.Items.Add("⚠ Low stock: Salt");
            lstLog.Items.Add("👤 Rediet clocked in");
            lstLog.Items.Add("✔ Order #1022 completed");
            lstLog.Items.Add("⚙ Backup completed");

            activityPanel.Controls.Add(lstLog);
        }

        // ================= CARD =================
        private void AddStatCard(FlowLayoutPanel flow, string title, string value, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(260, 150);
            card.BackColor = Color.White;
            card.Margin = new Padding(0, 0, 30, 30);

            UIHelper.SetRoundedRegion(card, 20);

            // Hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(250, 250, 250);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 11);
            lblTitle.ForeColor = Color.Gray;
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(20, 65);
            lblValue.AutoSize = true;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            flow.Controls.Add(card);
        }

        // ================= STYLES =================
        private void ApplyStyles()
        {
            UIHelper.ApplyTheme(this);

            UIHelper.ApplyModernButton(btnManageMenu, UIHelper.ControlColor);
            UIHelper.ApplyModernButton(btnReports, UIHelper.ControlColor);
            UIHelper.ApplyModernButton(btnStaff, UIHelper.ControlColor);
            UIHelper.ApplyModernButton(btnSettings, UIHelper.ControlColor);
            UIHelper.ApplyModernButton(btnLogout, Color.FromArgb(231, 76, 60));

            lblGreeting.Text = $"{UIHelper.GetGreeting()}, Admin";
            lblTime.Text = DateTime.Now.ToString("T");
            lblAdminTitle.Text = UIHelper.RestaurantName;
        }

        // ================= TIMERS =================
        private void clockTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("T");
        }

        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1)
                this.Opacity += 0.05;
            else
                fadeTimer.Stop();
        }

        // ================= NAVIGATION =================
        private void btnManageMenu_Click(object sender, EventArgs e)
        {
            LoadForm(new Menu_Form());
            lblPanelTitle.Text = "Menu Management";
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            LoadForm(new ReportForm());
            lblPanelTitle.Text = "Reports";
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            LoadForm(new StaffForm());
            lblPanelTitle.Text = "Staff Management";
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            LoadForm(new SettingsForm());
            lblPanelTitle.Text = "Settings";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            new LoginForm().Show();
        }

        private void LoadForm(Form form)
        {
            contentPanel.Controls.Clear();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(form);
            UIHelper.ApplyTheme(form);
            form.Show();
        }

        // ================= WINDOW =================
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            this.WindowState = this.WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }
    }
}