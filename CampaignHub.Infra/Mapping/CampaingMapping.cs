using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infra.Mappings;

public class CampaignMapping : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdAccountId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.CampaignStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.HasMany(x => x.Metrics)
            .WithOne()
            .HasForeignKey("CampaignId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}