using Microsoft.EntityFrameworkCore;
using TaskExpenseTracker12.Models;

namespace TaskExpenseTracker12.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TaskExpenseTracker12.Models.Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
