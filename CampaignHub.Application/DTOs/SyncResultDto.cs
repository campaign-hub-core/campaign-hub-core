namespace CampaignHub.Application.DTOs;

public record SyncResultDto(
    string AdAccountId,
    int CampaignsSynced,
    int AdSetsSynced,
    int AdsSynced,
    int MetricsSynced,
    DateTime SyncedAt,
    List<string> Errors
);
