using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampaignHub.Infras.Mappings;

public class ClientMapping : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Observation)
            .HasMaxLength(1000);

        builder.Property(x => x.ClientType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.ClientStatus)
            .IsRequired()
            .HasConversion<int>();
    }
}