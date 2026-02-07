using System.Diagnostics.CodeAnalysis;

namespace CampaignHub.Domain.Entities;

public class Organization : Entity
{
    public required string Name { get; set; }
    public bool Active { get; set; }

    protected Organization() { }

    [SetsRequiredMembers]
    public Organization(string nome)
    {
        Name = nome;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void Update(string name)
    {
        Name = name;
    }

    public void Disable()
    {
        Active = false;
    }
}
