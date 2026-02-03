using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class Campaign : Entity
{
    public string AdAccountId { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CampaignStatusEnum CampaignStatus { get; set; }

    protected Campaign() { }

    public Campaign(string adAccountId, string name, DateTime startDate, DateTime endDate)
    {
        AdAccountId = adAccountId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        CampaignStatus = CampaignStatusEnum.Active;
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
