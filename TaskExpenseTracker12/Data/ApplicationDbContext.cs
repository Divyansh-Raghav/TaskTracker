using Microsoft.EntityFrameworkCore;
using TaskExpenseTracker12.Models;

namespace TaskExpenseTracker12.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TaskExpenseTracker12.Models.Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Task entity
        modelBuilder.Entity<TaskExpenseTracker12.Models.Task>()
            .Property(t => t.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
