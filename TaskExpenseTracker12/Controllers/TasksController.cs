using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskExpenseTracker12.Data;
using TaskExpenseTracker12.Models;
using TaskEntity = TaskExpenseTracker12.Models.Task;
using TaskStatus = TaskExpenseTracker12.Models.TaskStatus;

namespace TaskExpenseTracker12.Controllers;

/// <summary>
/// Manages CRUD operations for tasks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all tasks.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
    {
        var tasks = await _context.Tasks.AsNoTracking().ToListAsync();
        return tasks.Select(ToDto).ToList();
    }

    /// <summary>
    /// Gets a task by identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(int id)
    {
        var task = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        return ToDto(task);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
    {
        var task = new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = TaskStatus.ToDo,
            AssignedUserId = dto.AssignedUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = ToDto(task);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, result);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        if (!IsValidStatusTransition(task.Status, dto.Status))
        {
            return BadRequest("Invalid status transition. Allowed transitions: ToDo -> InProgress -> Done.");
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.AssignedUserId = dto.AssignedUserId;
        task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return ToDto(task);
    }

    /// <summary>
    /// Deletes a task by identifier.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool IsValidStatusTransition(TaskStatus current, TaskStatus next)
    {
        if (current == next)
        {
            return true;
        }

        return (current == TaskStatus.ToDo && next == TaskStatus.InProgress)
            || (current == TaskStatus.InProgress && next == TaskStatus.Done);
    }

    private static TaskDto ToDto(TaskEntity task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        AssignedUserId = task.AssignedUserId,
        CreatedAt = task.CreatedAt
    };
}

/// <summary>
/// Response model for tasks.
/// </summary>
public class TaskDto
{
    /// <summary>Task identifier.</summary>
    public int Id { get; set; }

    /// <summary>Title of the task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional task description.</summary>
    public string? Description { get; set; }

    /// <summary>Current status of the task.</summary>
    public TaskStatus Status { get; set; }

    /// <summary>Identifier of the assigned user.</summary>
    public int? AssignedUserId { get; set; }

    /// <summary>Creation timestamp in UTC.</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Payload for creating tasks.
/// </summary>
public class CreateTaskDto
{
    /// <summary>Title of the task.</summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional task description.</summary>
    public string? Description { get; set; }

    /// <summary>Identifier of the assigned user.</summary>
    public int? AssignedUserId { get; set; }
}

/// <summary>
/// Payload for updating tasks.
/// </summary>
public class UpdateTaskDto
{
    /// <summary>Title of the task.</summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional task description.</summary>
    public string? Description { get; set; }

    /// <summary>Identifier of the assigned user.</summary>
    public int? AssignedUserId { get; set; }

    /// <summary>Desired status of the task.</summary>
    [Required]
    public TaskStatus Status { get; set; }
}
