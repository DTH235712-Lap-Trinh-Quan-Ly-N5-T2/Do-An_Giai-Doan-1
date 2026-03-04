using Microsoft.EntityFrameworkCore;
using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Infrastructure.Data;

namespace TaskFlowManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public TaskRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // ===============================
        // GET ALL (Read-Only Optimized)
        // ===============================
        public async Task<List<TaskItem>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.TaskItems
                .AsNoTracking() // 🔥 giảm memory tracking
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        // ===============================
        // PAGINATION (Quan trọng nhất)
        // ===============================
        public async Task<List<TaskItem>> GetTasksPagedAsync(int page, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.TaskItems
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // ===============================
        // GET BY ID
        // ===============================
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.TaskItems.FindAsync(id);
        }

        // ===============================
        // ADD
        // ===============================
        public async Task AddAsync(TaskItem task)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();
        }

        // ===============================
        // UPDATE
        // ===============================
        public async Task UpdateAsync(TaskItem task)
        {
            using var context = _contextFactory.CreateDbContext();

            context.TaskItems.Attach(task);
            context.Entry(task).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        // ===============================
        // DELETE
        // ===============================
        public async Task DeleteAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();

            var task = await context.TaskItems.FindAsync(id);
            if (task != null)
            {
                context.TaskItems.Remove(task);
                await context.SaveChangesAsync();
            }
        }
    }
}