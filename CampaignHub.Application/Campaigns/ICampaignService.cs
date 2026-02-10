namespace CampaignHub.Application.Campaigns;

public interface ICampaignService
{
    Task<CampaignResult> CreateAsync(CreateCampaignInput input, CancellationToken cancellationToken = default);
    Task<CampaignResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignResult>> GetByAdAccountIdAsync(string adAccountId, CancellationToken cancellationToken = default);
    Task<CampaignResult?> UpdateAsync(string id, UpdateCampaignInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<CampaignResult?> PauseAsync(string id, CancellationToken cancellationToken = default);
    Task<CampaignResult?> ActivateAsync(string id, CancellationToken cancellationToken = default);
    Task<CampaignResult?> CompleteAsync(string id, CancellationToken cancellationToken = default);
}
