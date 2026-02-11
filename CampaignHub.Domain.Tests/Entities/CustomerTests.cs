using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var customer = new Customer("Client A", "user-1", "Notes", CustomerTypeEnum.Lead);

        Assert.Equal("Client A", customer.Name);
        Assert.Equal("user-1", customer.UserId);
        Assert.Equal("Notes", customer.Observation);
        Assert.Equal(CustomerTypeEnum.Lead, customer.CustomerType);
        Assert.Equal(CustomerStatusEnum.Active, customer.CustomerStatus);
        Assert.NotNull(customer.Id);
        Assert.Empty(customer.AdAccounts);
    }

    [Fact]
    public void Constructor_WithNullObservation_ShouldAcceptNull()
    {
        var customer = new Customer("Client B", "user-2", null, CustomerTypeEnum.Ecommerce);

        Assert.Null(customer.Observation);
    }

    [Fact]
    public void Update_ShouldChangeNameAndObservation()
    {
        var customer = new Customer("Client A", "user-1", "Notes", CustomerTypeEnum.Lead);

        customer.Update("Client B", "New Notes");

        Assert.Equal("Client B", customer.Name);
        Assert.Equal("New Notes", customer.Observation);
    }

    [Fact]
    public void Enable_ShouldSetStatusActive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();

        customer.Enable();

        Assert.Equal(CustomerStatusEnum.Active, customer.CustomerStatus);
    }

    [Fact]
    public void Pause_ShouldSetStatusSuspended()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);

        customer.Pause();

        Assert.Equal(CustomerStatusEnum.Suspended, customer.CustomerStatus);
    }

    [Fact]
    public void Disable_ShouldSetStatusInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);

        customer.Disable();

        Assert.Equal(CustomerStatusEnum.Inactive, customer.CustomerStatus);
    }
}
