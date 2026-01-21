using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskExpenseTracker12.Data;
using TaskExpenseTracker12.Models;

namespace TaskExpenseTracker12.Controllers;

/// <summary>
/// Manages CRUD operations for users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        return users.Select(ToDto).ToList();
    }

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        return ToDto(user);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = ToDto(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, result);
    }

    /// <summary>
    /// Deletes a user by identifier.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name
    };
}

/// <summary>
/// Response model for users.
/// </summary>
public class UserDto
{
    /// <summary>User identifier.</summary>
    public int Id { get; set; }

    /// <summary>Name of the user.</summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Payload for creating users.
/// </summary>
public class CreateUserDto
{
    /// <summary>Name of the user.</summary>
    [Required]
    public string Name { get; set; } = string.Empty;
}
