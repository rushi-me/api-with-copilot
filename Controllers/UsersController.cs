using Microsoft.AspNetCore.Mvc;
using copilot_api.Models;
using copilot_api.Services;
using Microsoft.Extensions.Logging;

namespace copilot_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), 200)]
    public ActionResult<IEnumerable<UserResponseDto>> GetUsers()
    {
        try
        {
            var users = _userService.GetAllUsers();
            var response = users.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponseDto), 200)]
    [ProducesResponseType(404)]
    public ActionResult<UserResponseDto> GetUser(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            return Ok(MapToResponseDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="userDto">User creation data</param>
    /// <returns>Created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), 201)]
    [ProducesResponseType(400)]
    public ActionResult<UserResponseDto> CreateUser(CreateUserDto userDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, error) = _userService.CreateUser(userDto);
            
            if (error != null)
            {
                return BadRequest(new { error });
            }

            if (user == null)
            {
                return StatusCode(500, "Failed to create user");
            }

            var response = MapToResponseDto(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="userDto">Updated user data</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<UserResponseDto> UpdateUser(int id, UpdateUserDto userDto)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, error) = _userService.UpdateUser(id, userDto);
            
            if (error != null)
            {
                if (error.Contains("not found"))
                {
                    return NotFound($"User with ID {id} not found");
                }
                return BadRequest(new { error });
            }

            if (user == null)
            {
                return StatusCode(500, "Failed to update user");
            }

            return Ok(MapToResponseDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public ActionResult DeleteUser(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            var deleted = _userService.DeleteUser(id);
            if (!deleted)
            {
                return NotFound($"User with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return StatusCode(500, "An error occurred while deleting the user");
        }
    }

    /// <summary>
    /// Check if email is unique
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>Email uniqueness status</returns>
    [HttpGet("check-email")]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult CheckEmailUniqueness([FromQuery] string email, [FromQuery] int? excludeUserId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required");
            }

            var isUnique = _userService.IsEmailUnique(email, excludeUserId);
            return Ok(new { email, isUnique });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email uniqueness for {Email}", email);
            return StatusCode(500, "An error occurred while checking email uniqueness");
        }
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Department = user.Department,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        };
    }
} 