namespace TaskExpenseTracker12.Models;

public enum TaskStatus { ToDo = 0, InProgress = 1, Done = 2 }

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public int? AssignedUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Task> AssignedTasks { get; set; } = new();
}
