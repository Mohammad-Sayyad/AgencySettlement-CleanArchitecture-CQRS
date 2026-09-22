using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations;

internal sealed class PercentConfiguration
    : IEntityTypeConfiguration<Percent>
{
    public void Configure(EntityTypeBuilder<Percent> builder)
    {
        builder.ToTable("Percents", table =>
        {
            table.HasCheckConstraint(
                "CK_Percent_Total",
                "[AgencyPercent] + [GajPercent] + [StudentPercent] = 100");

            table.HasCheckConstraint(
                "CK_Percent_AgencyPercent",
                "[AgencyPercent] >= 0 AND [AgencyPercent] <= 100");

            table.HasCheckConstraint(
                "CK_Percent_GajPercent",
                "[GajPercent] >= 0 AND [GajPercent] <= 100");

            table.HasCheckConstraint(
                "CK_Percent_StudentPercent",
                "[StudentPercent] >= 0 AND [StudentPercent] <= 100");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AgencyId)
            .IsRequired();

        builder.Property(x => x.ExamModeId)
            .IsRequired();

        builder.Property(x => x.AgencyPercent)
            .HasPrecision(7, 4)
            .IsRequired();

        builder.Property(x => x.GajPercent)
            .HasPrecision(7, 4)
            .IsRequired();

        builder.Property(x => x.StudentPercent)
            .HasPrecision(7, 4)
            .IsRequired();

        builder.Property(x => x.PersianExecutionDate)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.YearId)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.AgencyId,
            x.ExamModeId,
            x.YearId,
            x.PersianExecutionDate,
            x.IsActive
        })
        .HasDatabaseName("IX_Percent_Match");
    }
}