namespace CampaignHub.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Active { get; set; }

    public User() {}

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void Disable()
    {
        Active = false;
    }   
}
