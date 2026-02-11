using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.Customers.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<Customer>> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await _context.Customers
            .AsNoTracking()
            .Where(c => c.Name.ToLower().Contains(name.ToLower()) && c.CustomerStatus != CustomerStatusEnum.Inactive)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Customer>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
        await _context.Customers
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.CustomerStatus != CustomerStatusEnum.Inactive)
            .ToListAsync(cancellationToken);

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        return customer;
    }
}
