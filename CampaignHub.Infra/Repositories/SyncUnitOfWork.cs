namespace CampaignHub.Infra.Repositories;

public class SyncUnitOfWork : Domain.Interfaces.IUnitOfWork
{
    private readonly AppDbContext _context;

    public SyncUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
