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
    public sealed class SettlementPaymentConfiguration
     : IEntityTypeConfiguration<SettlementPayment>
    {
        public void Configure(EntityTypeBuilder<SettlementPayment> builder)
        {
            builder.ToTable("SettlementPayments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SettlementId)
                .IsRequired();

            builder.Property(x => x.AgencyId)
                .IsRequired();

            builder.Property(x => x.YearId)
                .IsRequired();

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(19, 4)
                .IsRequired();

            builder.Property(x => x.ExamAmount)
                .HasPrecision(19, 4)
                .IsRequired()
                .HasDefaultValue(0m);

            builder.Property(x => x.SingleLessonAmount)
                .HasPrecision(19, 4)
                .IsRequired()
                .HasDefaultValue(0m);

            builder.Property(x => x.SingleLessonSettlementId)
                .IsRequired(false);

            builder.Property(x => x.PaymentDate)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(x => x.TrackingNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsAppliedToSettlement)
                .IsRequired()
                .HasDefaultValue(false);

            // رابطه با تسویه آزمون
            builder.HasOne(x => x.Settlement)
      .WithMany(x => x.Payments)
      .HasForeignKey(x => x.SettlementId)
      .OnDelete(DeleteBehavior.Restrict);

            // رابطه با نمایندگی
            builder.HasOne(x => x.Agency)
                .WithMany()
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // رابطه اختیاری با تسویه تک‌درس
            builder.HasOne(x => x.SingleLessonSettlement)
                .WithMany()
                .HasForeignKey(x => x.SingleLessonSettlementId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // ایندکس برای جست‌وجوی پرداخت‌ها
            builder.HasIndex(x => new
            {
                x.AgencyId,
                x.YearId,
                x.PersianExecutionDate
            });

            // جست‌وجوی شماره پیگیری
            builder.HasIndex(x => new
            {
                x.AgencyId,
                x.YearId,
                x.TrackingNumber
            });
        }
    }
}
