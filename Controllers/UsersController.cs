using Microsoft.AspNetCore.Mvc;
using copilot_api.Models;
using copilot_api.Services;

namespace copilot_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName) || 
            string.IsNullOrWhiteSpace(user.LastName) || 
            string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest("FirstName, LastName, and Email are required.");
        }

        var createdUser = _userService.CreateUser(user);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public ActionResult<User> UpdateUser(int id, User user)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName) || 
            string.IsNullOrWhiteSpace(user.LastName) || 
            string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest("FirstName, LastName, and Email are required.");
        }

        var updatedUser = _userService.UpdateUser(id, user);
        if (updatedUser == null)
            return NotFound();

        return Ok(updatedUser);
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        var deleted = _userService.DeleteUser(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
} 