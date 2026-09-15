using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations
{
    internal sealed class SettlementHistoryConfiguration
        : IEntityTypeConfiguration<SettlementHistory>
    {
        public void Configure(
            EntityTypeBuilder<SettlementHistory> builder)
        {
            builder.ToTable("SettlementHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalDebit)
                .HasPrecision(19, 4);

            builder.Property(x => x.TotalCredit)
                .HasPrecision(19, 4);

            builder.Property(x => x.Balance)
                .HasPrecision(19, 4);

            builder.Property(x => x.TotalDebitGaj)
                .HasPrecision(19, 4);

            builder.Property(x => x.TotalCreditGaj)
                .HasPrecision(19, 4);

            builder.Property(x => x.BalanceGaj)
                .HasPrecision(19, 4);

            builder.Property(x => x.ContractFloorAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.AgencyId,
                x.CreatedAt
            })
            .HasDatabaseName(
                "IX_SettlementHistories_Agency_Date");
        }
    }
}