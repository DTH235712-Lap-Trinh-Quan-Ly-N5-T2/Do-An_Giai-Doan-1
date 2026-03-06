using Microsoft.EntityFrameworkCore;
using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Infrastructure.Data
{
    /// <summary>
    /// AppDbContext – cầu nối giữa C# entities và SQL Server database.
    /// 
    /// Entity Framework Core sẽ:
    ///   - Tự động sinh ra SQL từ LINQ query
    ///   - Quản lý connection pool
    ///   - Apply migration để tạo/cập nhật bảng
    ///
    /// Cấu hình quan hệ (Fluent API) đặt trong OnModelCreating thay vì Data Annotations
    /// để giữ entities gọn, dễ đọc.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // =====================================================
        // DbSets – mỗi property tương ứng 1 bảng trong DB
        // =====================================================
        public DbSet<User>          Users          { get; set; }
        public DbSet<Role>          Roles          { get; set; }
        public DbSet<UserRole>      UserRoles      { get; set; } // Bảng trung gian User ↔ Role
        public DbSet<Customer>      Customers      { get; set; }
        public DbSet<Project>       Projects       { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; } // Bảng trung gian Project ↔ User
        public DbSet<Expense>       Expenses       { get; set; }
        public DbSet<Category>      Categories     { get; set; } // Lookup: phân loại task
        public DbSet<Priority>      Priorities     { get; set; } // Lookup: mức ưu tiên
        public DbSet<Status>        Statuses       { get; set; } // Lookup: trạng thái task
        public DbSet<TaskItem>      TaskItems      { get; set; }
        public DbSet<Comment>       Comments       { get; set; }
        public DbSet<Attachment>    Attachments    { get; set; }
        public DbSet<Tag>           Tags           { get; set; }
        public DbSet<TaskTag>       TaskTags       { get; set; } // Bảng trung gian Task ↔ Tag

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // COMPOSITE KEYS – khóa chính ghép cho bảng trung gian
            // =====================================================

            // UserRole: 1 user không thể có cùng 1 role 2 lần
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // TaskTag: 1 task không thể gán cùng 1 tag 2 lần
            modelBuilder.Entity<TaskTag>()
                .HasKey(tt => new { tt.TaskItemId, tt.TagId });

            // =====================================================
            // USER RELATIONSHIPS
            // =====================================================

            // User ↔ Role (nhiều-nhiều qua UserRole)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User).WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User → xóa UserRole liên quan

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role).WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index để đảm bảo email và username không trùng
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();

            // =====================================================
            // PROJECT RELATIONSHIPS
            // =====================================================

            // Project → Owner (1 User sở hữu nhiều Project)
            // Restrict: không xóa được User nếu còn là Owner của Project
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner).WithMany(u => u.OwnedProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project → Customer (1 Customer có nhiều Project)
            // SetNull: xóa Customer thì Project.CustomerId = null (project vẫn giữ lại)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Customer).WithMany(c => c.Projects)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // ProjectMember: Cascade khi xóa Project, Restrict khi xóa User
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project).WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User).WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 user chỉ xuất hiện 1 lần trong mỗi project (unique constraint)
            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => new { pm.ProjectId, pm.UserId }).IsUnique();

            // =====================================================
            // TASK RELATIONSHIPS
            // =====================================================

            // Task → CreatedBy và AssignedTo đều Restrict (không xóa User đang có task)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.CreatedBy).WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedTo).WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing: Task → SubTask (cây task lồng nhau)
            // Restrict: không xóa task cha khi còn task con
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.ParentTask).WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // EXPENSE RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Project).WithMany(p => p.Expenses)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Project → xóa Expense

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.CreatedBy).WithMany(u => u.CreatedExpenses)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // ATTACHMENT & TAG RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.UploadedBy).WithMany()
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.TaskItem).WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.Tag).WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================================
            // DATABASE INDEXES – TỐI ƯU QUERY PERFORMANCE
            // Thêm index cho các cột thường xuất hiện trong WHERE/ORDER BY
            // =====================================================

            // TaskItem – các cột filter phổ biến nhất
            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.ProjectId)
                .HasDatabaseName("IX_TaskItems_ProjectId");      // WHERE ProjectId = ?

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.StatusId)
                .HasDatabaseName("IX_TaskItems_StatusId");       // WHERE StatusId = ? (Kanban)

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.PriorityId)
                .HasDatabaseName("IX_TaskItems_PriorityId");     // WHERE PriorityId = ?

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.AssignedToId)
                .HasDatabaseName("IX_TaskItems_AssignedToId");   // WHERE AssignedToId = ? (My Tasks)

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.DueDate)
                .HasDatabaseName("IX_TaskItems_DueDate");        // Overdue / DueSoon query

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.CreatedAt)
                .HasDatabaseName("IX_TaskItems_CreatedAt");      // ORDER BY CreatedAt DESC

            // Composite index: tối ưu query filter cả project VÀ status cùng lúc (Kanban)
            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => new { t.ProjectId, t.StatusId })
                .HasDatabaseName("IX_TaskItems_ProjectId_StatusId");

            // Project indexes
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.OwnerId)
                .HasDatabaseName("IX_Projects_OwnerId");         // GetByOwnerAsync

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Status)
                .HasDatabaseName("IX_Projects_Status");          // GetActiveProjectsAsync

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.CustomerId)
                .HasDatabaseName("IX_Projects_CustomerId");      // Filter theo khách hàng

            // Comment index – load nhanh bình luận của 1 task
            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.TaskItemId)
                .HasDatabaseName("IX_Comments_TaskItemId");

            // Expense index – tính tổng chi phí theo project
            modelBuilder.Entity<Expense>()
                .HasIndex(e => e.ProjectId)
                .HasDatabaseName("IX_Expenses_ProjectId");

            // ProjectMember index – GetByMemberAsync
            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => pm.UserId)
                .HasDatabaseName("IX_ProjectMembers_UserId");
        }
    }
}
