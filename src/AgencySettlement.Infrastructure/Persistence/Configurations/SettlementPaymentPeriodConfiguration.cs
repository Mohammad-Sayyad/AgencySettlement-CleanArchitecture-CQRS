using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations;

internal sealed class SettlementPaymentPeriodConfiguration
    : IEntityTypeConfiguration<SettlementPaymentPeriod>
{
    public void Configure(
        EntityTypeBuilder<SettlementPaymentPeriod> builder)
    {
        builder.ToTable("SettlementPaymentPeriods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SettlementId)
            .IsRequired();

        builder.Property(x => x.AgencyId)
            .IsRequired();

        builder.Property(x => x.YearId)
            .IsRequired();

        builder.Property(x => x.PersianExecutionDate)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PaymentStartDate)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PaymentDeadlineDate)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<Settlement>()
            .WithMany()
            .HasForeignKey(x => x.SettlementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SettlementId)
            .IsUnique()
            .HasDatabaseName(
                "UX_SettlementPaymentPeriods_SettlementId");

        builder.HasIndex(x => new
        {
            x.AgencyId,
            x.YearId,
            x.PersianExecutionDate
        })
        .HasDatabaseName(
            "IX_SettlementPaymentPeriods_Agency_Year_Date");
    }
}