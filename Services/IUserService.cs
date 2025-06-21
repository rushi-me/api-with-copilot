using copilot_api.Models;

namespace copilot_api.Services;

public interface IUserService
{
    IEnumerable<User> GetAllUsers();
    User? GetUserById(int id);
    User CreateUser(User user);
    User? UpdateUser(int id, User user);
    bool DeleteUser(int id);
} 