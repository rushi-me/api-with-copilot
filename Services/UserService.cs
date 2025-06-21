using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using copilot_api.Models;

namespace copilot_api.Services;

public class UserService : IUserService
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private readonly ConcurrentDictionary<string, int> _emailToIdMap = new();
    private int _nextId = 1;
    private readonly object _idLock = new object();

    public UserService()
    {
        // Initialize with 2 hardcoded records
        var user1 = new User
        {
            Id = GetNextId(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@company.com",
            Department = "Engineering",
            CreatedAt = DateTime.Now.AddDays(-30),
            IsActive = true
        };

        var user2 = new User
        {
            Id = GetNextId(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@company.com",
            Department = "Marketing",
            CreatedAt = DateTime.Now.AddDays(-15),
            IsActive = true
        };

        _users.TryAdd(user1.Id, user1);
        _users.TryAdd(user2.Id, user2);
        _emailToIdMap.TryAdd(user1.Email.ToLowerInvariant(), user1.Id);
        _emailToIdMap.TryAdd(user2.Email.ToLowerInvariant(), user2.Id);
    }

    private int GetNextId()
    {
        lock (_idLock)
        {
            return _nextId++;
        }
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

    public User? GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var normalizedEmail = email.ToLowerInvariant();
        if (_emailToIdMap.TryGetValue(normalizedEmail, out var userId))
        {
            _users.TryGetValue(userId, out var user);
            return user;
        }
        return null;
    }

    public bool IsEmailUnique(string email, int? excludeUserId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var normalizedEmail = email.ToLowerInvariant();
        
        if (!_emailToIdMap.TryGetValue(normalizedEmail, out var existingUserId))
            return true;

        // If we're updating a user, allow them to keep their current email
        return excludeUserId.HasValue && existingUserId == excludeUserId.Value;
    }

    public (User? user, string? error) CreateUser(CreateUserDto userDto)
    {
        try
        {
            // Validate email format
            if (!IsValidEmail(userDto.Email))
            {
                return (null, "Invalid email format");
            }

            // Check email uniqueness
            if (!IsEmailUnique(userDto.Email))
            {
                return (null, "Email already exists");
            }

            var user = new User
            {
                Id = GetNextId(),
                FirstName = userDto.FirstName.Trim(),
                LastName = userDto.LastName.Trim(),
                Email = userDto.Email.Trim().ToLowerInvariant(),
                Department = userDto.Department.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = userDto.IsActive
            };

            if (!_users.TryAdd(user.Id, user))
            {
                return (null, "Failed to create user - ID conflict");
            }

            if (!_emailToIdMap.TryAdd(user.Email, user.Id))
            {
                // Rollback user creation if email mapping fails
                _users.TryRemove(user.Id, out _);
                return (null, "Failed to create user - email mapping conflict");
            }

            return (user, null);
        }
        catch (Exception ex)
        {
            return (null, $"An error occurred while creating user: {ex.Message}");
        }
    }

    public (User? user, string? error) UpdateUser(int id, UpdateUserDto userDto)
    {
        try
        {
            if (!_users.TryGetValue(id, out var existingUser))
            {
                return (null, "User not found");
            }

            // Validate email format
            if (!IsValidEmail(userDto.Email))
            {
                return (null, "Invalid email format");
            }

            var normalizedEmail = userDto.Email.Trim().ToLowerInvariant();

            // Check email uniqueness (excluding current user)
            if (!IsEmailUnique(normalizedEmail, id))
            {
                return (null, "Email already exists");
            }

            // Remove old email mapping if email changed
            if (existingUser.Email != normalizedEmail)
            {
                _emailToIdMap.TryRemove(existingUser.Email, out _);
            }

            var updatedUser = new User
            {
                Id = id,
                FirstName = userDto.FirstName.Trim(),
                LastName = userDto.LastName.Trim(),
                Email = normalizedEmail,
                Department = userDto.Department.Trim(),
                CreatedAt = existingUser.CreatedAt, // Preserve original creation date
                IsActive = userDto.IsActive
            };

            _users[id] = updatedUser;
            _emailToIdMap[normalizedEmail] = id;

            return (updatedUser, null);
        }
        catch (Exception ex)
        {
            return (null, $"An error occurred while updating user: {ex.Message}");
        }
    }

    public bool DeleteUser(int id)
    {
        try
        {
            if (_users.TryGetValue(id, out var user))
            {
                // Remove email mapping
                _emailToIdMap.TryRemove(user.Email, out _);
                // Remove user
                return _users.TryRemove(id, out _);
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Basic email validation regex
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
} 