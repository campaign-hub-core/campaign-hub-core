using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Infra.Repositories;

namespace CampaignHub.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerResult> CreateAsync(CreateCustomerInput input, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId, cancellationToken);
        if (user is null || !user.Active)
            throw new InvalidOperationException("O usuário informado não existe ou está inativo.");

        var customer = new Customer(input.Name.Trim(), input.UserId, input.Observation?.Trim(), input.CustomerType);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(customer);
    }

    public async Task<CustomerResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null || customer.CustomerStatus == CustomerStatusEnum.Inactive)
            return null;

        return ToResult(customer);
    }

    public async Task<IEnumerable<CustomerResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetByNameAsync(name, cancellationToken);
        return customers.Select(ToResult);
    }

    public async Task<CustomerResult?> UpdateAsync(string id, UpdateCustomerInput input, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null || customer.CustomerStatus == CustomerStatusEnum.Inactive)
            return null;

        customer.Update(input.Name.Trim(), input.Observation?.Trim());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(customer);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null || customer.CustomerStatus == CustomerStatusEnum.Inactive)
            return false;

        customer.Disable();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<CustomerResult?> EnableAsync(string id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
            return null;

        customer.Enable();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(customer);
    }

    public async Task<CustomerResult?> PauseAsync(string id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null || customer.CustomerStatus == CustomerStatusEnum.Inactive)
            return null;

        customer.Pause();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(customer);
    }

    public async Task<CustomerResult?> DisableAsync(string id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null || customer.CustomerStatus == CustomerStatusEnum.Inactive)
            return null;

        customer.Disable();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(customer);
    }

    private static CustomerResult ToResult(Customer customer)
    {
        return new CustomerResult(
            customer.Id,
            customer.Name,
            customer.UserId,
            customer.Observation,
            customer.CustomerType,
            customer.CustomerStatus,
            customer.CreatedAt
        );
    }
}
