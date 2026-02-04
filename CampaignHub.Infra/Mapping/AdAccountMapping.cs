using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infra.Mapping;

public class AdAccountMapping : IEntityTypeConfiguration<AdAccount>
{
    public void Configure(EntityTypeBuilder<AdAccount> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClientId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MonthlyBudget)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Goal)
            .HasMaxLength(500);

        builder.Property(x => x.AdPlatform)
            .IsRequired()
            .HasConversion<int>();
    }
}