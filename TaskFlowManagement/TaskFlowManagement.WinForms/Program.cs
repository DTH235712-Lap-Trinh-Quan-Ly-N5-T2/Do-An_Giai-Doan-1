using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Infrastructure.Data;
using TaskFlowManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace TaskFlowManagement.WinForms
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.EnableRetryOnFailure()
                ));

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<Form1>();

            ServiceProvider = services.BuildServiceProvider();

            // ================= SEED HERE =================
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var context = factory.CreateDbContext();
                DbSeeder.SeedAsync(context).GetAwaiter().GetResult();
            }
            // =============================================

            ApplicationConfiguration.Initialize();

            var form = ServiceProvider.GetRequiredService<Form1>();
            System.Windows.Forms.Application.Run(form);
        }
    }
}