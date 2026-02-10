using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Infra.Repositories;

namespace CampaignHub.Application.Campaigns;

public class CampaignService : ICampaignService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IAdAccountRepository _adAccountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CampaignService(ICampaignRepository campaignRepository, IAdAccountRepository adAccountRepository, IUnitOfWork unitOfWork)
    {
        _campaignRepository = campaignRepository;
        _adAccountRepository = adAccountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignResult> CreateAsync(CreateCampaignInput input, CancellationToken cancellationToken = default)
    {
        var adAccount = await _adAccountRepository.GetByIdAsync(input.AdAccountId, cancellationToken);
        if (adAccount is null)
            throw new InvalidOperationException("A conta de anúncios informada não existe.");

        if (input.StartDate >= input.EndDate)
            throw new InvalidOperationException("A data de início deve ser anterior à data de término.");

        var campaign = new Campaign(input.AdAccountId, input.Name.Trim(), input.StartDate, input.EndDate);

        await _campaignRepository.AddAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(campaign);
    }

    public async Task<CampaignResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return null;

        return ToResult(campaign);
    }

    public async Task<IEnumerable<CampaignResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var campaigns = await _campaignRepository.GetByNameAsync(name, cancellationToken);
        return campaigns.Select(ToResult);
    }

    public async Task<IEnumerable<CampaignResult>> GetByAdAccountIdAsync(string adAccountId, CancellationToken cancellationToken = default)
    {
        var campaigns = await _campaignRepository.GetByAdAccountIdAsync(adAccountId, cancellationToken);
        return campaigns.Select(ToResult);
    }

    public async Task<CampaignResult?> UpdateAsync(string id, UpdateCampaignInput input, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return null;

        if (campaign.CampaignStatus == CampaignStatusEnum.Completed || campaign.CampaignStatus == CampaignStatusEnum.Cancelled)
            throw new InvalidOperationException("Não é possível atualizar uma campanha concluída ou cancelada.");

        if (input.StartDate >= input.EndDate)
            throw new InvalidOperationException("A data de início deve ser anterior à data de término.");

        campaign.Update(input.Name.Trim(), input.StartDate, input.EndDate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(campaign);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return false;

        _campaignRepository.Remove(campaign);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<CampaignResult?> PauseAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return null;

        if (campaign.CampaignStatus == CampaignStatusEnum.Completed || campaign.CampaignStatus == CampaignStatusEnum.Cancelled)
            throw new InvalidOperationException("Não é possível pausar uma campanha concluída ou cancelada.");

        campaign.Pause();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(campaign);
    }

    public async Task<CampaignResult?> ActivateAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return null;

        if (campaign.CampaignStatus == CampaignStatusEnum.Completed || campaign.CampaignStatus == CampaignStatusEnum.Cancelled)
            throw new InvalidOperationException("Não é possível ativar uma campanha concluída ou cancelada.");

        campaign.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(campaign);
    }

    public async Task<CampaignResult?> CompleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return null;

        campaign.Complete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(campaign);
    }

    private static CampaignResult ToResult(Campaign campaign)
    {
        return new CampaignResult(
            campaign.Id,
            campaign.AdAccountId,
            campaign.Name,
            campaign.StartDate,
            campaign.EndDate,
            campaign.CampaignStatus,
            campaign.CreatedAt
        );
    }
}
