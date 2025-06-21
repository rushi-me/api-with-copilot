using copilot_api.Models;

namespace copilot_api.Services;

public interface IUserService
{
    IEnumerable<User> GetAllUsers();
    User? GetUserById(int id);
    User? GetUserByEmail(string email);
    (User? user, string? error) CreateUser(CreateUserDto userDto);
    (User? user, string? error) UpdateUser(int id, UpdateUserDto userDto);
    bool DeleteUser(int id);
    bool IsEmailUnique(string email, int? excludeUserId = null);
} 