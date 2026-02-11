using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Tests.Entities;

public class MetricCampaignTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var period = new DateTime(2026, 3, 15);

        var metric = new MetricCampaign("camp-1", period, 1500.50m, 25, "10", "5000.00");

        Assert.Equal("camp-1", metric.CampaignId);
        Assert.Equal(1500.50m, metric.Expenses);
        Assert.Equal(25, metric.Leads);
        Assert.Equal("10", metric.Sales);
        Assert.Equal("5000.00", metric.Revenue);
        Assert.NotNull(metric.Id);
    }

    [Fact]
    public void Constructor_ShouldNormalizePeriodToFirstDayOfMonth()
    {
        var period = new DateTime(2026, 3, 15);

        var metric = new MetricCampaign("camp-1", period, 100m, 5, null, null);

        Assert.Equal(new DateTime(2026, 3, 1), metric.ReferencePeriod);
    }

    [Fact]
    public void Constructor_WhenAlreadyFirstDay_ShouldKeepDate()
    {
        var period = new DateTime(2026, 5, 1);

        var metric = new MetricCampaign("camp-1", period, 100m, 5, null, null);

        Assert.Equal(new DateTime(2026, 5, 1), metric.ReferencePeriod);
    }

    [Fact]
    public void Update_ShouldChangeAllMetrics()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, "2", "500");

        metric.Update(200m, 10, "5", "1000");

        Assert.Equal(200m, metric.Expenses);
        Assert.Equal(10, metric.Leads);
        Assert.Equal("5", metric.Sales);
        Assert.Equal("1000", metric.Revenue);
    }

    [Fact]
    public void UpdateExpenses_ShouldChangeOnlyExpenses()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, "2", "500");

        metric.UpdateExpenses(300m);

        Assert.Equal(300m, metric.Expenses);
        Assert.Equal(5, metric.Leads);
        Assert.Equal("2", metric.Sales);
        Assert.Equal("500", metric.Revenue);
    }

    [Fact]
    public void Constructor_WithNullSalesAndRevenue_ShouldAcceptNull()
    {
        var metric = new MetricCampaign("camp-1", DateTime.Now, 100m, 5, null, null);

        Assert.Null(metric.Sales);
        Assert.Null(metric.Revenue);
    }
}
