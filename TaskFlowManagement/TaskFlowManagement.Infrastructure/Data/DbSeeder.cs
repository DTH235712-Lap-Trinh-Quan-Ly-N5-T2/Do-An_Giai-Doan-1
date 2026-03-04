using TaskFlowManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace TaskFlowManagement.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Nếu đã có user thì coi như đã seed
            if (await context.Users.AnyAsync())
                return;

            var random = new Random();
            var now = DateTime.UtcNow;

            // =====================================================
            // 1️⃣ LOOKUP TABLES (Seed trước để tránh lỗi FK)
            // =====================================================

            var priorities = new List<Priority>
            {
                new Priority { Name = "Low" },
                new Priority { Name = "Medium" },
                new Priority { Name = "High" }
            };

            var statuses = new List<Status>
            {
                new Status { Name = "Todo" },
                new Status { Name = "In Progress" },
                new Status { Name = "Done" }
            };

            var categories = new List<Category>
            {
                new Category { Name = "Bug" },
                new Category { Name = "Feature" },
                new Category { Name = "Improvement" }
            };

            var tags = new List<Tag>
            {
                new Tag { Name = "Urgent" },
                new Tag { Name = "UI" },
                new Tag { Name = "Backend" },
                new Tag { Name = "Database" },
                new Tag { Name = "API" }
            };

            context.AddRange(priorities);
            context.AddRange(statuses);
            context.AddRange(categories);
            context.AddRange(tags);

            await context.SaveChangesAsync();

            var priorityIds = priorities.Select(p => p.Id).ToList();
            var statusIds = statuses.Select(s => s.Id).ToList();
            var categoryIds = categories.Select(c => c.Id).ToList();
            var tagIds = tags.Select(t => t.Id).ToList();

            // =====================================================
            // 2️⃣ USERS
            // =====================================================

            var users = Enumerable.Range(1, 15)
                .Select(i => new User
                {
                    FullName = $"User {i}",
                    Email = $"user{i}@test.com",
                    PasswordHash = "123",
                    IsActive = true
                }).ToList();

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            var userIds = users.Select(u => u.Id).ToList();

            // =====================================================
            // 3️⃣ PROJECTS
            // =====================================================

            var projects = Enumerable.Range(1, 10)
                .Select(i => new Project
                {
                    Name = $"Project {i}",
                    OwnerId = userIds[random.Next(userIds.Count)]
                }).ToList();

            context.Projects.AddRange(projects);
            await context.SaveChangesAsync();

            var projectIds = projects.Select(p => p.Id).ToList();

            // =====================================================
            // 4️⃣ TASKS
            // =====================================================

            var tasks = new List<TaskItem>();

            for (int i = 1; i <= 200; i++) // tăng lên 200 để test stress
            {
                tasks.Add(new TaskItem
                {
                    Title = $"Task {i}",
                    Description = $"Description for task {i}",
                    ProjectId = projectIds[random.Next(projectIds.Count)],
                    CreatedById = userIds[random.Next(userIds.Count)],
                    AssignedToId = random.Next(0, 3) == 0
                        ? null // tạo task chưa assign
                        : userIds[random.Next(userIds.Count)],
                    PriorityId = priorityIds[random.Next(priorityIds.Count)],
                    StatusId = statusIds[random.Next(statusIds.Count)],
                    CategoryId = categoryIds[random.Next(categoryIds.Count)],
                    CreatedAt = now.AddDays(-random.Next(1, 60)),
                    DueDate = now.AddDays(random.Next(-15, 30)) // có overdue
                });
            }

            context.TaskItems.AddRange(tasks);
            await context.SaveChangesAsync();

            var taskIds = tasks.Select(t => t.Id).ToList();

            // =====================================================
            // 5️⃣ COMMENTS
            // =====================================================

            var comments = Enumerable.Range(1, 400)
                .Select(i => new Comment
                {
                    Content = $"Comment {i}",
                    TaskItemId = taskIds[random.Next(taskIds.Count)],
                    UserId = userIds[random.Next(userIds.Count)],
                    CreatedAt = now.AddDays(-random.Next(1, 30))
                }).ToList();

            context.Comments.AddRange(comments);

            // =====================================================
            // 6️⃣ ATTACHMENTS
            // =====================================================

            var attachments = Enumerable.Range(1, 150)
                .Select(i => new Attachment
                {
                    FileName = $"file{i}.txt",
                    FilePath = $"/uploads/file{i}.txt",
                    TaskItemId = taskIds[random.Next(taskIds.Count)]
                }).ToList();

            context.Attachments.AddRange(attachments);

            // =====================================================
            // 7️⃣ TASK TAGS (TRÁNH DUPLICATE COMPOSITE KEY)
            // =====================================================

            var taskTagSet = new HashSet<(int TaskId, int TagId)>();

            while (taskTagSet.Count < 400)
            {
                taskTagSet.Add((
                    taskIds[random.Next(taskIds.Count)],
                    tagIds[random.Next(tagIds.Count)]
                ));
            }

            var taskTags = taskTagSet.Select(tt => new TaskTag
            {
                TaskItemId = tt.TaskId,
                TagId = tt.TagId
            });

            context.TaskTags.AddRange(taskTags);

            await context.SaveChangesAsync();
        }
    }
}