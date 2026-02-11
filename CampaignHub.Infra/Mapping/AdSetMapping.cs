using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infra.Mapping;

public class AdSetMapping : IEntityTypeConfiguration<AdSet>
{
    public void Configure(EntityTypeBuilder<AdSet> builder)
    {
        builder.ToTable("AdSet");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CampaignId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ExternalId)
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.DailyBudget)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasFilter("\"ExternalId\" IS NOT NULL");

        builder.HasMany(x => x.Ads)
            .WithOne()
            .HasForeignKey("AdSetId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
