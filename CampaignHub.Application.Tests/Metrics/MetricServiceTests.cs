using CampaignHub.Application.Metrics;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using NSubstitute;

namespace CampaignHub.Application.Tests.Metrics;

public class MetricServiceTests
{
    private readonly IMetricCampaignRepository _metricRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MetricService _sut;

    public MetricServiceTests()
    {
        _metricRepository = Substitute.For<IMetricCampaignRepository>();
        _campaignRepository = Substitute.For<ICampaignRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new MetricService(_metricRepository, _campaignRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateMetric_WhenValid()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);
        _metricRepository.GetByCampaignAndPeriodAsync(campaign.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((MetricCampaign?)null);
        _metricRepository.AddAsync(Arg.Any<MetricCampaign>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<MetricCampaign>());
        var input = new CreateMetricInput(campaign.Id, new DateTime(2026, 3, 15), 1500m, 25, "10", "5000");

        var result = await _sut.CreateAsync(input);

        Assert.Equal(campaign.Id, result.CampaignId);
        Assert.Equal(1500m, result.Expenses);
        Assert.Equal(25, result.Leads);
        Assert.Equal("10", result.Sales);
        Assert.Equal("5000", result.Revenue);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCampaignNotFound()
    {
        _campaignRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);
        var input = new CreateMetricInput("nonexistent", DateTime.Now, 100m, 5, null, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenExpensesNegative()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);
        var input = new CreateMetricInput(campaign.Id, DateTime.Now, -100m, 5, null, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenLeadsNegative()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);
        var input = new CreateMetricInput(campaign.Id, DateTime.Now, 100m, -5, null, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDuplicatePeriod()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        _campaignRepository.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>())
            .Returns(campaign);
        var existingMetric = new MetricCampaign(campaign.Id, DateTime.Now, 100m, 5, null, null);
        _metricRepository.GetByCampaignAndPeriodAsync(campaign.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(existingMetric);
        var input = new CreateMetricInput(campaign.Id, DateTime.Now, 200m, 10, null, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMetric_WhenExists()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, null, null);
        _metricRepository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>())
            .Returns(metric);

        var result = await _sut.GetByIdAsync(metric.Id);

        Assert.NotNull(result);
        Assert.Equal(metric.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _metricRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((MetricCampaign?)null);

        var result = await _sut.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCampaignIdAsync_ShouldReturnMetrics()
    {
        var metrics = new List<MetricCampaign>
        {
            new("camp-1", new DateTime(2026, 1, 1), 100m, 5, null, null),
            new("camp-1", new DateTime(2026, 2, 1), 200m, 10, null, null)
        };
        _metricRepository.GetByCampaignIdAsync("camp-1", Arg.Any<CancellationToken>())
            .Returns(metrics);

        var result = await _sut.GetByCampaignIdAsync("camp-1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMetric_WhenExists()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, "2", "500");
        _metricRepository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>())
            .Returns(metric);

        var result = await _sut.UpdateAsync(metric.Id, new UpdateMetricInput(200m, 10, " 5 ", " 1000 "));

        Assert.NotNull(result);
        Assert.Equal(200m, result!.Expenses);
        Assert.Equal(10, result.Leads);
        Assert.Equal("5", result.Sales);
        Assert.Equal("1000", result.Revenue);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound()
    {
        _metricRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((MetricCampaign?)null);

        var result = await _sut.UpdateAsync("nonexistent", new UpdateMetricInput(100m, 5, null, null));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenExpensesNegative()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, null, null);
        _metricRepository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>())
            .Returns(metric);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(metric.Id, new UpdateMetricInput(-100m, 5, null, null)));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenLeadsNegative()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, null, null);
        _metricRepository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>())
            .Returns(metric);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(metric.Id, new UpdateMetricInput(100m, -5, null, null)));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveMetric_WhenExists()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, null, null);
        _metricRepository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>())
            .Returns(metric);

        var result = await _sut.DeleteAsync(metric.Id);

        Assert.True(result);
        _metricRepository.Received(1).Remove(metric);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _metricRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((MetricCampaign?)null);

        var result = await _sut.DeleteAsync("nonexistent");

        Assert.False(result);
    }
}
