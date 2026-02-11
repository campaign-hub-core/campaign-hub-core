using System.Diagnostics.CodeAnalysis;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class AdSet : Entity
{
    public required string CampaignId { get; set; }
    public required string Name { get; set; }
    public string? ExternalId { get; set; }
    public AdSetStatusEnum Status { get; set; }
    public decimal DailyBudget { get; set; }

    private readonly List<Ad> _ads = new();
    public IReadOnlyCollection<Ad> Ads => _ads;

    protected AdSet() { }

    [SetsRequiredMembers]
    public AdSet(string campaignId, string name, AdSetStatusEnum status, decimal dailyBudget)
    {
        CampaignId = campaignId;
        Name = name;
        Status = status;
        DailyBudget = dailyBudget;
    }

    public void Pause()
    {
        Status = AdSetStatusEnum.Paused;
    }

    public void Activate()
    {
        Status = AdSetStatusEnum.Active;
    }

    public void SetExternalId(string externalId)
    {
        ExternalId = externalId;
    }
}
