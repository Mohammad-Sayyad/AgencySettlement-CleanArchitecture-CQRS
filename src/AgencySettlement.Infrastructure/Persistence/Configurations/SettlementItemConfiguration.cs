using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations
{
    internal sealed class SettlementItemConfiguration
        : IEntityTypeConfiguration<SettlementItem>
    {
        public void Configure(
            EntityTypeBuilder<SettlementItem> builder)
        {
            builder.ToTable("SettlementItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UnitPrice)
                .HasPrecision(19, 4);

            builder.Property(x => x.BaseAmount)
                .HasPrecision(19, 4);

            builder.Property(x => x.AgencyPercent)
                .HasPrecision(7, 4);

            builder.Property(x => x.GajPercent)
                .HasPrecision(7, 4);

            builder.Property(x => x.StudentPercent)
                .HasPrecision(7, 4);

            builder.Property(x => x.AgencyAmount)
                .HasPrecision(19, 4);

            builder.Property(x => x.GajAmount)
                .HasPrecision(19, 4);

            builder.Property(x => x.StudentAmount)
                .HasPrecision(19, 4);

            builder.Property(x => x.DebitAmount)
                .HasPrecision(19, 4);

            builder.Property(x => x.CreditAmount)
                .HasPrecision(19, 4);

            builder.Property(x => x.FreeCandidateCount)
                .IsRequired();

            builder.Property(x => x.PaidCandidateCount)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.SettlementId,
                x.RegistrationPlanId,
                x.StudyFieldId
            })
            .HasDatabaseName("IX_SettlementItems_Settlement");
        }
    }
}