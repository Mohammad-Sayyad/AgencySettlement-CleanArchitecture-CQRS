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
    public sealed class SettlementDebtConfiguration
    : IEntityTypeConfiguration<SettlementDebt>
    {
        public void Configure(
            EntityTypeBuilder<SettlementDebt> builder)
        {
            builder.ToTable("SettlementDebts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.AgencyId)
                .IsRequired();

            builder.Property(x => x.YearId)
                .IsRequired();

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(19, 4)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Agency)
                .WithMany()
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.AgencyId,
                x.YearId,
                x.PersianExecutionDate
            })
            .IsUnique();
        }
}

}
