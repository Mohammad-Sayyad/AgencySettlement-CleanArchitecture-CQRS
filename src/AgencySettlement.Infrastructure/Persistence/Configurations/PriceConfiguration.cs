using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations;

internal sealed class PriceConfiguration
    : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.ToTable("Prices", table =>
        {
            table.HasCheckConstraint(
                "CK_Prices_Amount",
                "[Amount] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PackageId)
            .IsRequired();

        builder.Property(x => x.EducationalLevelId)
            .IsRequired();

        builder.Property(x => x.ExamModeId)
            .IsRequired();

        builder.Property(x => x.RegistrationPlanId)
            .IsRequired();

        builder.Property(x => x.YearId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(x => x.PersianExecutionDate)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.PackageId,
            x.EducationalLevelId,
            x.ExamModeId,
            x.RegistrationPlanId,
            x.YearId,
            x.PersianExecutionDate,
            x.IsActive
        })
        .HasDatabaseName("IX_Prices_Match");
    }
}