namespace CampaignHub.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Active { get; set; }

    public User() {}

    public User(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void Disable()
    {
        Active = false;
    }
}
