using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Infrastructure.Data.Seed
{
    /// <summary>
    /// Seed dữ liệu cho các bảng Lookup:
    /// Roles, Priorities, Statuses, Categories, Tags.
    ///
    /// Tách riêng để dễ thêm/sửa lookup value mà không ảnh hưởng seed user/project.
    /// </summary>
    internal static class LookupSeeder
    {
        internal static List<Role> GetRoles() => new()
        {
            new() { Name = "Admin",     Description = "Toàn quyền hệ thống" },
            new() { Name = "Manager",   Description = "Quản lý dự án, khách hàng, báo cáo" },
            new() { Name = "Developer", Description = "Thực hiện công việc được giao" }
        };

        internal static List<Priority> GetPriorities() => new()
        {
            new() { Name = "Low",      Level = 1, ColorHex = "#4CAF50" }, // Xanh lá
            new() { Name = "Medium",   Level = 2, ColorHex = "#FF9800" }, // Cam
            new() { Name = "High",     Level = 3, ColorHex = "#F44336" }, // Đỏ
            new() { Name = "Critical", Level = 4, ColorHex = "#9C27B0" }  // Tím
        };

        internal static List<Status> GetStatuses() => new()
        {
            new() { Name = "Todo",       DisplayOrder = 0, ColorHex = "#9E9E9E" }, // Xám
            new() { Name = "InProgress", DisplayOrder = 1, ColorHex = "#2196F3" }, // Xanh dương
            new() { Name = "InReview",   DisplayOrder = 2, ColorHex = "#FF9800" }, // Cam
            new() { Name = "Done",       DisplayOrder = 3, ColorHex = "#4CAF50" }  // Xanh lá
        };

        internal static List<Category> GetCategories() => new()
        {
            new() { Name = "Bug",         Description = "Lỗi cần sửa" },
            new() { Name = "Feature",     Description = "Tính năng mới" },
            new() { Name = "Improvement", Description = "Cải tiến hiện có" },
            new() { Name = "Research",    Description = "Nghiên cứu kỹ thuật" },
            new() { Name = "Testing",     Description = "Kiểm thử" }
        };

        internal static List<Tag> GetTags() => new()
        {
            new() { Name = "Urgent" },    new() { Name = "UI" },
            new() { Name = "Backend" },   new() { Name = "Database" },
            new() { Name = "API" },       new() { Name = "Security" },
            new() { Name = "Performance" }
        };
    }
}
