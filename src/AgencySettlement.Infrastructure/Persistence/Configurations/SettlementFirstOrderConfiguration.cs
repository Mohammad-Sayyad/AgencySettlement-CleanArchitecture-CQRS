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
    public sealed class SettlementFirstOrderConfiguration
    : IEntityTypeConfiguration<SettlementFirstOrder>
    {
        public void Configure(
            EntityTypeBuilder<SettlementFirstOrder> builder)
        {
            builder.ToTable("SettlementFirstOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.TotalDebit)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalCredit)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Balance)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalDebitGaj)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalCreditGaj)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.BalanceGaj)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ContractFloorAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
