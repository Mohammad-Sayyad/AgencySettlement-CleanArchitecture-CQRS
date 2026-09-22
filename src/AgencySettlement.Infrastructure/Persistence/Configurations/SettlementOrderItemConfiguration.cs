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
    public sealed class SettlementOrderItemConfiguration
     : IEntityTypeConfiguration<SettlementOrderItem>
    {
        public void Configure(EntityTypeBuilder<SettlementOrderItem> builder)
        {
            builder.ToTable("SettlementOrderItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContractFloorAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.BaseAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.AgencyPercent)
                .HasPrecision(18, 2);

            builder.Property(x => x.GajPercent)
                .HasPrecision(18, 2);

            builder.Property(x => x.StudentPercent)
                .HasPrecision(18, 2);

            builder.Property(x => x.AgencyAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.GajAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.StudentAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.DebitAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.CreditAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => x.SettlementOrderId);
        }
    }
}
