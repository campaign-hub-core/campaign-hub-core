using CampaignHub.Application.Campaigns;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using NSubstitute;

namespace CampaignHub.Application.Tests.Campaigns;

public class CampaignServiceTests
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IAdAccountRepository _adAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CampaignService _sut;

    public CampaignServiceTests()
    {
        _campaignRepository = Substitute.For<ICampaignRepository>();
        _adAccountRepository = Substitute.For<IAdAccountRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new CampaignService(_campaignRepository, _adAccountRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCampaign_WhenValid()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);
        _campaignRepository.AddAsync(Arg.Any<Campaign>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Campaign>());
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 12, 31);
        var input = new CreateCampaignInput(adAccount.Id, " Summer Sale ", start, end);

        var result = await _sut.CreateAsync(input);

        Assert.Equal("Summer Sale", result.Name);
        Assert.Equal(start, result.StartDate);
        Assert.Equal(end, result.EndDate);
        Assert.Equal(CampaignStatusEnum.Active, result.CampaignStatus);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenAdAccountNotFound()
    {
        _adAccountRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((AdAccount?)null);
        var input = new CreateCampaignInput("nonexistent", "Name", DateTime.Now, DateTime.Now.AddDays(30));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStartDateIsAfterEndDate()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);
        var input = new CreateCampaignInput(adAccount.Id, "Name", DateTime.Now.AddDays(30), DateTime.Now);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStartDateEqualsEndDate()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepository.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);
        var sameDate = DateTime.Now;
        var input = new CreateCampaignInput(adAccount.Id, "Name", sameDate, sameDate);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCampaign_WhenExists()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        var result = await _sut.GetByIdAsync(campaign.Id);

        Assert.NotNull(result);
        Assert.Equal(campaign.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _campaignRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);

        var result = await _sut.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnMatchingCampaigns()
    {
        var campaigns = new List<Campaign>
        {
            new("acc-1", "Summer", DateTime.Now, DateTime.Now.AddDays(30)),
            new("acc-1", "Summer Sale", DateTime.Now, DateTime.Now.AddDays(60))
        };
        _campaignRepository.GetByNameAsync("Summer", Arg.Any<CancellationToken>())
            .Returns(campaigns);

        var result = await _sut.GetByNameAsync("Summer");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByAdAccountIdAsync_ShouldReturnCampaigns()
    {
        var campaigns = new List<Campaign>
        {
            new("acc-1", "Campaign 1", DateTime.Now, DateTime.Now.AddDays(30)),
            new("acc-1", "Campaign 2", DateTime.Now, DateTime.Now.AddDays(60))
        };
        _campaignRepository.GetByAdAccountIdAsync("acc-1", Arg.Any<CancellationToken>())
            .Returns(campaigns);

        var result = await _sut.GetByAdAccountIdAsync("acc-1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCampaign_WhenActiveAndValid()
    {
        var campaign = new Campaign("acc-1", "Old", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);
        var newStart = new DateTime(2026, 6, 1);
        var newEnd = new DateTime(2026, 6, 30);

        var result = await _sut.UpdateAsync(campaign.Id, new UpdateCampaignInput(" New Name ", newStart, newEnd));

        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenCampaignIsCompleted()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        campaign.Complete();
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(campaign.Id, new UpdateCampaignInput("New", DateTime.Now, DateTime.Now.AddDays(30))));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenInvalidDates()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(campaign.Id, new UpdateCampaignInput("New", DateTime.Now.AddDays(30), DateTime.Now)));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCampaign_WhenExists()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        var result = await _sut.DeleteAsync(campaign.Id);

        Assert.True(result);
        _campaignRepository.Received(1).Remove(campaign);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _campaignRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);

        var result = await _sut.DeleteAsync("nonexistent");

        Assert.False(result);
    }

    [Fact]
    public async Task PauseAsync_ShouldPauseCampaign_WhenActive()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        var result = await _sut.PauseAsync(campaign.Id);

        Assert.NotNull(result);
        Assert.Equal(CampaignStatusEnum.Paused, result!.CampaignStatus);
    }

    [Fact]
    public async Task PauseAsync_ShouldThrow_WhenCompleted()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        campaign.Complete();
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.PauseAsync(campaign.Id));
    }

    [Fact]
    public async Task PauseAsync_ShouldReturnNull_WhenNotFound()
    {
        _campaignRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);

        var result = await _sut.PauseAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task ActivateAsync_ShouldActivateCampaign_WhenPaused()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        campaign.Pause();
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        var result = await _sut.ActivateAsync(campaign.Id);

        Assert.NotNull(result);
        Assert.Equal(CampaignStatusEnum.Active, result!.CampaignStatus);
    }

    [Fact]
    public async Task ActivateAsync_ShouldThrow_WhenCompleted()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        campaign.Complete();
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ActivateAsync(campaign.Id));
    }

    [Fact]
    public async Task CompleteAsync_ShouldCompleteCampaign()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);

        var result = await _sut.CompleteAsync(campaign.Id);

        Assert.NotNull(result);
        Assert.Equal(CampaignStatusEnum.Completed, result!.CampaignStatus);
    }

    [Fact]
    public async Task CompleteAsync_ShouldReturnNull_WhenNotFound()
    {
        _campaignRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);

        var result = await _sut.CompleteAsync("nonexistent");

        Assert.Null(result);
    }
}
