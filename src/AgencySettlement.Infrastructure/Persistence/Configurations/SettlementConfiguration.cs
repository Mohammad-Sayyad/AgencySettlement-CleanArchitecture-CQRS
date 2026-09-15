using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations
{
    internal sealed class SettlementConfiguration
        : IEntityTypeConfiguration<Settlement>
    {
        public void Configure(
            EntityTypeBuilder<Settlement> builder)
        {
            builder.ToTable("Settlements");

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

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.SettlementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.AgencyId,
                x.YearId
            })
            .HasDatabaseName("IX_Settlements_Agency_Year");
        }
    }
}