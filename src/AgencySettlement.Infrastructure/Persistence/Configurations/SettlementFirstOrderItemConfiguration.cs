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
    public sealed class SettlementFirstOrderPaymentConfiguration
    : IEntityTypeConfiguration<SettlementFirstOrderPayment>
    {
        public void Configure(
            EntityTypeBuilder<SettlementFirstOrderPayment> builder)
        {
            builder.ToTable("SettlementFirstOrderPayments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaymentDate)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.TrackingNumber)
                .HasMaxLength(200);

            builder.Property(x => x.PaymentReference)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.SettlementFirstOrder)
                .WithMany()
                .HasForeignKey(x => x.SettlementFirstOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
