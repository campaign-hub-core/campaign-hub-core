namespace CampaignHub.Domain.Entities;

public class Organization : Entity
{
    public string Name { get; set; }
    public bool Active { get; set; }

    protected Organization() { }

    public Organization(string nome)
    {
        Name = nome;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void Disable()
    {
        Active = false;
    }
}
