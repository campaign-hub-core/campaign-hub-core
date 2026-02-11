using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infra.Mapping;

public class AdAccountMapping : IEntityTypeConfiguration<AdAccount>
{
    public void Configure(EntityTypeBuilder<AdAccount> builder)
    {
        builder.ToTable("AdAccount");
  
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
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

        builder.Property(x => x.ExternalId)
            .HasMaxLength(100);

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasFilter("\"ExternalId\" IS NOT NULL");

        builder.HasMany(x => x.Campaigns)
            .WithOne()
            .HasForeignKey("AdAccountId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}