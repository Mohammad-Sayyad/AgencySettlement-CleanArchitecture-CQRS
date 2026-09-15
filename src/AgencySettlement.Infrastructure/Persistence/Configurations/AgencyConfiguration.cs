using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations;

internal sealed class AgencyConfiguration
    : IEntityTypeConfiguration<Agency>
{
    public void Configure(
        EntityTypeBuilder<Agency> builder)
    {
        builder.ToTable("Agencies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DetailCode)
            .IsRequired();

        builder.Property(x => x.FreeQuotaCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.OneHundredThousandQuotaCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.ContractFloorAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Region)
            .WithMany(x => x.Agencies)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}