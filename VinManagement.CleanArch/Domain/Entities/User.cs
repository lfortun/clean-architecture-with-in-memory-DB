using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "User";

    private User()
    {
    }

    public User(string username, string passwordHash, string role = "User")
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}
