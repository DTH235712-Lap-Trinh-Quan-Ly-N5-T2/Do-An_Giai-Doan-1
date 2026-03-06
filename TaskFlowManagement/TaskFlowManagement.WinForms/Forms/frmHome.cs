using TaskFlowManagement.WinForms.Common;

namespace TaskFlowManagement.WinForms.Forms
{
    /// <summary>
    /// Màn hình chào sau khi đăng nhập thành công.
    /// Hiển thị: tên user, vai trò, thống kê nhanh (Giai đoạn 2 sẽ gắn data thật).
    /// Là MDI Child đầu tiên tự động mở khi vào frmMain.
    /// </summary>
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
            LoadWelcomeInfo();
        }

        private void LoadWelcomeInfo()
        {
            // Lấy giờ hiện tại để chọn lời chào phù hợp
            var hour = DateTime.Now.Hour;
            var greeting = hour < 12 ? "Chào buổi sáng" :
                           hour < 18 ? "Chào buổi chiều" : "Chào buổi tối";

            lblGreeting.Text  = $"{greeting}, {AppSession.FullName}! 👋";
            lblRole.Text      = $"Vai trò: {string.Join(", ", AppSession.Roles)}";
            lblLastLogin.Text = $"Đăng nhập lúc: {DateTime.Now:HH:mm  dd/MM/yyyy}";

            // Placeholder stats – Giai đoạn 2 sẽ gọi Service để lấy số thật
            lblStatProjects.Text = "—";
            lblStatTasks.Text    = "—";
            lblStatOverdue.Text  = "—";
            lblStatDone.Text     = "—";
        }
    }
}
