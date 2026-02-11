using System.Diagnostics.CodeAnalysis;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class Ad : Entity
{
    public required string AdSetId { get; set; }
    public required string Name { get; set; }
    public string? ExternalId { get; set; }
    public AdStatusEnum Status { get; set; }

    protected Ad() { }

    [SetsRequiredMembers]
    public Ad(string adSetId, string name, AdStatusEnum status)
    {
        AdSetId = adSetId;
        Name = name;
        Status = status;
    }

    public void Pause()
    {
        Status = AdStatusEnum.Paused;
    }

    public void Activate()
    {
        Status = AdStatusEnum.Active;
    }

    public void SetExternalId(string externalId)
    {
        ExternalId = externalId;
    }
}
