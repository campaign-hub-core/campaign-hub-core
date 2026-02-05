using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class Customer : Entity
{
    public required string UserId { get; set; }
    public string? Observation { get; set; }
    public CustomerTypeEnum CustomerType { get; set; }
    public CustomerStatusEnum CustomerStatus { get; set; }

    private readonly List<AdAccount> _adAccounts = new();
    public IReadOnlyCollection<AdAccount> AdAccounts => _adAccounts;

    protected Customer() { }

    public Customer(string userId, string observation, CustomerTypeEnum customerType)
    {
        UserId = userId;
        Observation = observation;
        CustomerType = customerType;
        CustomerStatus = CustomerStatusEnum.Active;
        CreatedAt = DateTime.UtcNow;
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
