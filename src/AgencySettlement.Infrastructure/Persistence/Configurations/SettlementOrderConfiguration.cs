using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Configurations
{
    public sealed class SettlementOrderConfiguration
     : IEntityTypeConfiguration<SettlementOrder>
    {
        public void Configure(EntityTypeBuilder<SettlementOrder> builder)
        {
            builder.ToTable("SettlementOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContractFloorAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.TotalDebit)
                .HasPrecision(18, 2);

            builder.Property(x => x.TotalCredit)
                .HasPrecision(18, 2);

            builder.Property(x => x.Balance)
                .HasPrecision(18, 2);

            builder.Property(x => x.TotalDebitGaj)
                .HasPrecision(18, 2);

            builder.Property(x => x.TotalCreditGaj)
                .HasPrecision(18, 2);

            builder.Property(x => x.BalanceGaj)
                .HasPrecision(18, 2);

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.AgencyId,
                x.YearId,
                x.RegistrationOrder
            });

            builder.HasMany(x => x.Items)
                .WithOne(x => x.SettlementOrder)
                .HasForeignKey(x => x.SettlementOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
