using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Core.Interfaces.Services;
using TaskFlowManagement.Core.Services.Auth;     // AuthService
using TaskFlowManagement.Core.Services.Users;    // UserService    (số nhiều – tránh xung đột Entity User)
using TaskFlowManagement.Core.Services.Projects; // ProjectService (số nhiều – tránh xung đột Entity Project)
using TaskFlowManagement.Core.Services.Tasks;    // TaskService    (số nhiều – tránh xung đột C# Task<T>)
using TaskFlowManagement.Infrastructure.Data;
using TaskFlowManagement.Infrastructure.Repositories;
using TaskFlowManagement.WinForms.Forms;

namespace TaskFlowManagement.WinForms
{
    /// <summary>
    /// Entry point của ứng dụng – cấu hình DI Container và khởi động.
    ///
    /// Thứ tự đăng ký dịch vụ:
    ///   1. Configuration  (đọc appsettings.json)
    ///   2. DbContext      (EF Core + SQL Server)
    ///   3. Repositories   (Data Access Layer)
    ///   4. Services       (Business Logic Layer)
    ///   5. Forms          (Presentation Layer)
    ///
    /// Sau đó: Migrate DB → Seed data → Mở Login → Mở Main
    /// </summary>
    internal static class Program
    {
        // ServiceProvider toàn cục – dùng để resolve Form ở cuối Main()
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            var services = new ServiceCollection();

            // =================================================
            // 1. CONFIGURATION – đọc appsettings.json
            // =================================================
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            services.AddSingleton<IConfiguration>(config);

            // =================================================
            // 2. DATABASE – dùng IDbContextFactory thay vì AddDbContext
            //
            // Lý do: WinForms không có "request scope" như ASP.NET Web
            //   AddDbContext → 1 DbContext dùng chung → conflict khi nhiều Form
            //   IDbContextFactory → mỗi Repository tạo DbContext riêng khi cần
            // =================================================
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        // Tự retry 3 lần khi SQL Server tạm thời lỗi (mạng, restart...)
                        sql.EnableRetryOnFailure(maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                        sql.CommandTimeout(30); // Timeout sau 30 giây
                    })
                // NoTracking mặc định: SELECT không cần EF theo dõi thay đổi → nhẹ hơn
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            // =================================================
            // 3. REPOSITORIES – Data Access Layer
            // Scoped: 1 instance trong 1 scope, đồng bộ lifecycle với DbContext
            // =================================================
            services.AddScoped<IUserRepository,     UserRepository>();
            services.AddScoped<IProjectRepository,  ProjectRepository>();
            services.AddScoped<ITaskRepository,     TaskRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // =================================================
            // 4. SERVICES – Business Logic Layer
            // Scoped: cùng scope với Repository để inject đúng instance
            // Namespace số nhiều: tránh xung đột tên với Entity class và C# keywords
            // =================================================
            services.AddScoped<IAuthService,    AuthService>();    // Services/Auth/
            services.AddScoped<IUserService,    UserService>();    // Services/Users/
            services.AddScoped<IProjectService, ProjectService>(); // Services/Projects/
            services.AddScoped<ITaskService,    TaskService>();    // Services/Tasks/

            // =================================================
            // 5. FORMS – Presentation Layer
            // Transient: Form mới mỗi lần resolve → không tái dùng state cũ
            // =================================================
            services.AddTransient<frmLogin>();
            services.AddTransient<frmMain>();
            services.AddTransient<frmHome>();

            ServiceProvider = services.BuildServiceProvider();

            // =================================================
            // MIGRATE + SEED – chạy khi khởi động app
            // context.Database.Migrate(): tạo DB nếu chưa có, apply migration mới
            // DbSeeder.SeedAsync(): chỉ seed khi DB còn trống (idempotent)
            // =================================================
            try
            {
                using var scope   = ServiceProvider.CreateScope();
                var factory       = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var context = factory.CreateDbContext();
                context.Database.Migrate();
                DbSeeder.SeedAsync(context).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi chi tiết kèm hướng dẫn xử lý 3 nguyên nhân phổ biến
                MessageBox.Show(
                    $"❌ Không thể kết nối database:\n\n{ex.Message}\n\n" +
                    "Kiểm tra:\n" +
                    "  1. SQL Server Express đang chạy\n" +
                    "     (SQL Server Configuration Manager → SQL Server Services)\n" +
                    "  2. Tên instance trong appsettings.json đúng không\n" +
                    "     (mặc định: SQLEXPRESS2022)\n" +
                    "  3. Windows Firewall không chặn SQL Server",
                    "Lỗi kết nối Database",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Thoát app nếu không có DB
            }

            // =================================================
            // KHỞI ĐỘNG: Login → (nếu OK) → Main
            // =================================================
            var loginForm = ServiceProvider.GetRequiredService<frmLogin>();

            // ShowDialog: chờ form Login đóng mới chạy tiếp
            // DialogResult.OK = đăng nhập thành công (set trong frmLogin.cs)
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // AppSession đã có data từ AuthService.LoginAsync()
                var mainForm = ServiceProvider.GetRequiredService<frmMain>();
                Application.Run(mainForm); // Vòng lặp message pump chính của WinForms
            }
            // Nếu đóng Login không đăng nhập → app thoát tự nhiên
        }
    }
}
