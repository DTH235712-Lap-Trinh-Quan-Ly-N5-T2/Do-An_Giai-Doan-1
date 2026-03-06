using Microsoft.Extensions.DependencyInjection;
using TaskFlowManagement.WinForms.Common;

namespace TaskFlowManagement.WinForms.Forms
{
    public partial class frmMain : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private System.Windows.Forms.Timer _clockTimer = null!;

        public frmMain(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            ApplyRolePermissions();
            StartClock();
            UpdateUserInfo();
        }

        // ── Mở frmHome làm child form đầu tiên khi app khởi động ──
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var home = new frmHome
            {
                MdiParent    = this,
                WindowState  = FormWindowState.Maximized
            };
            home.Show();
        }

        // ── Cập nhật thông tin user lên statusbar ─────────────────
        private void UpdateUserInfo()
        {
            lblStatusUser.Text = $"  👤 {AppSession.FullName}";
            lblStatusRole.Text = $"  [{string.Join(", ", AppSession.Roles)}]";
            this.Text          = $"TaskFlow  —  {AppSession.FullName}";
        }

        // ── Ẩn/hiện menu theo Role ────────────────────────────────
        private void ApplyRolePermissions()
        {
            if (!AppSession.IsManager)
            {
                menuUsers.Visible     = false;
                menuCustomers.Visible = false;
                menuReports.Visible   = false;
                menuTaskList.Visible  = false;
                menuKanban.Visible    = false;
            }

            if (!AppSession.IsAdmin)
                menuUserAccounts.Visible = false;
        }

        // ── Đồng hồ realtime ──────────────────────────────────────
        private void StartClock()
        {
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) =>
                lblStatusTime.Text = DateTime.Now.ToString("HH:mm:ss   dd/MM/yyyy  ");
            _clockTimer.Start();
        }

        // ── Menu: Hệ thống ────────────────────────────────────────
        private void menuChangePassword_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tính năng đổi mật khẩu sẽ có ở Giai đoạn 2.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận đăng xuất",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _clockTimer?.Stop();
            AppSession.Logout();
            this.Hide();

            var loginForm = _serviceProvider.GetRequiredService<frmLogin>();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                UpdateUserInfo();
                ApplyRolePermissions();
                StartClock();
                this.Show();
            }
            else
            {
                Application.Exit();
            }
        }

        private void menuExit_Click(object sender, EventArgs e) => Application.Exit();

        // ── Menu: Người dùng ──────────────────────────────────────
        private void menuUserAccounts_Click(object sender, EventArgs e)
            => ShowComingSoon("Quản lý tài khoản", "Giai đoạn 3");

        private void menuEmployees_Click(object sender, EventArgs e)
            => ShowComingSoon("Quản lý nhân viên", "Giai đoạn 3");

        // ── Menu: Khách hàng ──────────────────────────────────────
        private void menuCustomerList_Click(object sender, EventArgs e)
            => ShowComingSoon("Danh sách khách hàng", "Giai đoạn 4");

        // ── Menu: Dự án ───────────────────────────────────────────
        private void menuProjectList_Click(object sender, EventArgs e)
            => ShowComingSoon("Danh sách dự án", "Giai đoạn 2");

        private void menuProjectNew_Click(object sender, EventArgs e)
            => ShowComingSoon("Tạo dự án mới", "Giai đoạn 2");

        // ── Menu: Công việc ───────────────────────────────────────
        private void menuTaskList_Click(object sender, EventArgs e)
            => ShowComingSoon("Danh sách công việc", "Giai đoạn 2");

        private void menuKanban_Click(object sender, EventArgs e)
            => ShowComingSoon("Kanban Board", "Giai đoạn 3");

        private void menuMyTasks_Click(object sender, EventArgs e)
            => ShowComingSoon("Công việc của tôi", "Giai đoạn 2");

        // ── Menu: Báo cáo ─────────────────────────────────────────
        private void menuDashboard_Click(object sender, EventArgs e)
            => ShowComingSoon("Dashboard tổng quan", "Giai đoạn 2");

        private void menuReportProgress_Click(object sender, EventArgs e)
            => ShowComingSoon("Báo cáo tiến độ", "Giai đoạn 5");

        private void menuReportBudget_Click(object sender, EventArgs e)
            => ShowComingSoon("Báo cáo ngân sách", "Giai đoạn 5");

        // ── Helper: thông báo "Đang phát triển" ──────────────────
        private static void ShowComingSoon(string featureName, string phase)
        {
            MessageBox.Show(
                $"Tính năng  \"{featureName}\"  đang được phát triển.\n\n" +
                $"Dự kiến hoàn thành: {phase}.",
                "Đang phát triển",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ── Cleanup ───────────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _clockTimer?.Stop();
            _clockTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
