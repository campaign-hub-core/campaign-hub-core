using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;

namespace CampaignHub.Application.AdAccounts;

public class AdAccountService : IAdAccountService
{
    private readonly IAdAccountRepository _adAccountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdAccountService(IAdAccountRepository adAccountRepository, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _adAccountRepository = adAccountRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdAccountResult> CreateAsync(CreateAdAccountInput input, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(input.CustomerId, cancellationToken);
        if (customer is null || customer.CustomerStatus == CustomerStatusEnum.Inactive)
            throw new InvalidOperationException("O cliente informado não existe ou está inativo.");

        if (input.MonthlyBudget <= 0)
            throw new InvalidOperationException("O orçamento mensal deve ser maior que zero.");

        var adAccount = new AdAccount(input.CustomerId, input.MonthlyBudget, input.Goal.Trim(), input.AdPlatform);

        await _adAccountRepository.AddAsync(adAccount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(adAccount);
    }

    public async Task<AdAccountResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var adAccount = await _adAccountRepository.GetByIdAsync(id, cancellationToken);

        if (adAccount is null)
            return null;

        return ToResult(adAccount);
    }

    public async Task<IEnumerable<AdAccountResult>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        var adAccounts = await _adAccountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return adAccounts.Select(ToResult);
    }

    public async Task<AdAccountResult?> UpdateAsync(string id, UpdateAdAccountInput input, CancellationToken cancellationToken = default)
    {
        var adAccount = await _adAccountRepository.GetByIdAsync(id, cancellationToken);

        if (adAccount is null)
            return null;

        if (input.MonthlyBudget <= 0)
            throw new InvalidOperationException("O orçamento mensal deve ser maior que zero.");

        adAccount.Update(input.MonthlyBudget, input.Goal.Trim());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(adAccount);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var adAccount = await _adAccountRepository.GetByIdAsync(id, cancellationToken);

        if (adAccount is null)
            return false;

        _adAccountRepository.Remove(adAccount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static AdAccountResult ToResult(AdAccount adAccount)
    {
        return new AdAccountResult(
            adAccount.Id,
            adAccount.CustomerId,
            adAccount.MonthlyBudget,
            adAccount.Goal,
            adAccount.AdPlatform,
            adAccount.CreatedAt
        );
    }
}
