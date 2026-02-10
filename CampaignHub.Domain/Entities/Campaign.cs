using System.Diagnostics.CodeAnalysis;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class Campaign : Entity
{
    public required string AdAccountId { get; set; }
    public required string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CampaignStatusEnum CampaignStatus { get; set; }

    private readonly List<MetricCampaign> _metrics = new();
    public IReadOnlyCollection<MetricCampaign> Metrics => _metrics;

    protected Campaign() { }

    [SetsRequiredMembers]
    public Campaign(string adAccountId, string name, DateTime startDate, DateTime endDate)
    {
        AdAccountId = adAccountId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        CampaignStatus = CampaignStatusEnum.Active;
    }

    public void Update(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Pause()
    {
        CampaignStatus = CampaignStatusEnum.Paused;
    }

    public void Activate()
    {
        CampaignStatus = CampaignStatusEnum.Active;
    }

    public void Complete()
    {
        CampaignStatus = CampaignStatusEnum.Completed;
    }
}
