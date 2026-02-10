namespace CampaignHub.Application.Campaigns;

public record CreateCampaignInput(string AdAccountId, string Name, DateTime StartDate, DateTime EndDate);
