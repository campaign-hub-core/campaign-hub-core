using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra;

public class AppDbContext : DbContext
{
    protected AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<MetricCampaign> MetricCampaigns { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<AdAccount> AdAccounts { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
