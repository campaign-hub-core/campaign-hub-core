using CampaignHub.Application.AdAccounts;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using NSubstitute;

namespace CampaignHub.Application.Tests.AdAccounts;

public class AdAccountServiceTests
{
    private readonly IAdAccountRepository _adAccountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AdAccountService _sut;

    public AdAccountServiceTests()
    {
        _adAccountRepository = Substitute.For<IAdAccountRepository>();
        _customerRepository = Substitute.For<ICustomerRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new AdAccountService(_adAccountRepository, _customerRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAdAccount_WhenCustomerExistsAndActive()
    {
        var customer = new Customer("Client", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);
        _adAccountRepository.AddAsync(Arg.Any<AdAccount>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<AdAccount>());
        var input = new CreateAdAccountInput(customer.Id, 5000m, " Sales Goal ", AdPlatformEnum.MetaAds);

        var result = await _sut.CreateAsync(input);

        Assert.Equal(customer.Id, result.CustomerId);
        Assert.Equal(5000m, result.MonthlyBudget);
        Assert.Equal("Sales Goal", result.Goal);
        Assert.Equal(AdPlatformEnum.MetaAds, result.AdPlatform);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCustomerNotFound()
    {
        _customerRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Customer?)null);
        var input = new CreateAdAccountInput("nonexistent", 5000m, "Goal", AdPlatformEnum.MetaAds);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCustomerInactive()
    {
        var customer = new Customer("Client", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);
        var input = new CreateAdAccountInput(customer.Id, 5000m, "Goal", AdPlatformEnum.MetaAds);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenBudgetIsZero()
    {
        var customer = new Customer("Client", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);
        var input = new CreateAdAccountInput(customer.Id, 0m, "Goal", AdPlatformEnum.MetaAds);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenBudgetIsNegative()
    {
        var customer = new Customer("Client", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);
        var input = new CreateAdAccountInput(customer.Id, -100m, "Goal", AdPlatformEnum.MetaAds);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAdAccount_WhenExists()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        var result = await _sut.GetByIdAsync(adAccount.Id);

        Assert.NotNull(result);
        Assert.Equal(adAccount.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _adAccountRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((AdAccount?)null);

        var result = await _sut.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ShouldReturnAccounts()
    {
        var accounts = new List<AdAccount>
        {
            new("cust-1", 5000m, "Goal 1", AdPlatformEnum.MetaAds),
            new("cust-1", 3000m, "Goal 2", AdPlatformEnum.GoogleAds)
        };
        _adAccountRepository.GetByCustomerIdAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(accounts);

        var result = await _sut.GetByCustomerIdAsync("cust-1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAdAccount_WhenExists()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Old Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        var result = await _sut.UpdateAsync(adAccount.Id, new UpdateAdAccountInput(10000m, " New Goal "));

        Assert.NotNull(result);
        Assert.Equal(10000m, result!.MonthlyBudget);
        Assert.Equal("New Goal", result.Goal);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound()
    {
        _adAccountRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((AdAccount?)null);

        var result = await _sut.UpdateAsync("nonexistent", new UpdateAdAccountInput(5000m, "Goal"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenBudgetIsZeroOrNegative()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(adAccount.Id, new UpdateAdAccountInput(0m, "Goal")));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAdAccount_WhenExists()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        var result = await _sut.DeleteAsync(adAccount.Id);

        Assert.True(result);
        _adAccountRepository.Received(1).Remove(adAccount);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _adAccountRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((AdAccount?)null);

        var result = await _sut.DeleteAsync("nonexistent");

        Assert.False(result);
    }
}
