using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infra.Mapping;

public class AdMapping : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        builder.ToTable("Ad");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdSetId)
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

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasFilter("\"ExternalId\" IS NOT NULL");
    }
}
