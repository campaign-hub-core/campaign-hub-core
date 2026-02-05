using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infrastructure.Mappings;

public class MetricCampaignMapping : IEntityTypeConfiguration<MetricCampaign>
{
    public void Configure(EntityTypeBuilder<MetricCampaign> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CampaignId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReferencePeriod)
            .IsRequired();

        builder.Property(x => x.Expenses)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Leads)
            .IsRequired();

        builder.Property(x => x.Sales)
            .HasMaxLength(100);

        builder.Property(x => x.Revenue)
            .HasMaxLength(100);

        builder.HasIndex("CampaignId", nameof(MetricCampaign.ReferenceEquals))
            .IsUnique();

        builder.HasOne<Campaign>()
            .WithMany(x => x.Metrics)
            .HasForeignKey("CampaignId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}