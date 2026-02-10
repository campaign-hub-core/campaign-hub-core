using System.Diagnostics.CodeAnalysis;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class Customer : Entity
{
    public required string Name { get; set; }
    public required string UserId { get; set; }
    public string? Observation { get; set; }
    public CustomerTypeEnum CustomerType { get; set; }
    public CustomerStatusEnum CustomerStatus { get; set; }

    private readonly List<AdAccount> _adAccounts = new();
    public IReadOnlyCollection<AdAccount> AdAccounts => _adAccounts;

    protected Customer() { }

    [SetsRequiredMembers]
    public Customer(string name, string userId, string? observation, CustomerTypeEnum customerType)
    {
        Name = name;
        UserId = userId;
        Observation = observation;
        CustomerType = customerType;
        CustomerStatus = CustomerStatusEnum.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string? observation)
    {
        Name = name;
        Observation = observation;
    }

    public void Enable()
    {
        CustomerStatus = CustomerStatusEnum.Active;
    }

    public void Pause()
    {
        CustomerStatus = CustomerStatusEnum.Suspended;
    }

    public void Disable()
    {
        CustomerStatus = CustomerStatusEnum.Inactive;
    }
}
