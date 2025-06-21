using System.Collections.Concurrent;
using copilot_api.Models;

namespace copilot_api.Services;

public class UserService : IUserService
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId = 1;

    public UserService()
    {
        // Initialize with 2 hardcoded records
        var user1 = new User
        {
            Id = _nextId++,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@company.com",
            Department = "Engineering",
            CreatedAt = DateTime.Now.AddDays(-30),
            IsActive = true
        };

        var user2 = new User
        {
            Id = _nextId++,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@company.com",
            Department = "Marketing",
            CreatedAt = DateTime.Now.AddDays(-15),
            IsActive = true
        };

        _users.TryAdd(user1.Id, user1);
        _users.TryAdd(user2.Id, user2);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _users.Values;
    }

    public User? GetUserById(int id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public User CreateUser(User user)
    {
        user.Id = _nextId++;
        user.CreatedAt = DateTime.Now;
        _users.TryAdd(user.Id, user);
        return user;
    }

    public User? UpdateUser(int id, User user)
    {
        if (!_users.ContainsKey(id))
            return null;

        user.Id = id;
        user.CreatedAt = _users[id].CreatedAt; // Preserve original creation date
        _users[id] = user;
        return user;
    }

    public bool DeleteUser(int id)
    {
        return _users.TryRemove(id, out _);
    }
} 