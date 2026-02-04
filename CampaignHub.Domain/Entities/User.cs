namespace CampaignHub.Domain.Entities;

public class User : Entity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
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

    public void Disable()
    {
        Active = false;
    }   
}
