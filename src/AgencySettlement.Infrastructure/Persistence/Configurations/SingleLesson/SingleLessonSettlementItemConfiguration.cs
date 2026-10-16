using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations.SingleLesson;

public sealed class SingleLessonSettlementItemConfiguration : IEntityTypeConfiguration<SingleLessonSettlementItem>
{
    public void Configure(EntityTypeBuilder<SingleLessonSettlementItem> b)
    {
        b.ToTable("SingleLessonSettlementItems", t => {
            t.HasCheckConstraint("CK_SingleLessonSettlementItems_Quantity", "[Quantity] > 0");
            t.HasCheckConstraint("CK_SingleLessonSettlementItems_Amounts", "[UnitPrice] >= 0 AND [BaseAmount] >= 0 AND [AgencyAmount] >= 0 AND [GajAmount] >= 0 AND [DebitAmount] >= 0 AND [CreditAmount] >= 0");
            t.HasCheckConstraint("CK_SingleLessonSettlementItems_PercentSum", "[AgencyPercent] >= 0 AND [GajPercent] >= 0 AND [AgencyPercent] + [GajPercent] = 100");
        });
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.PersianExecutionDate).HasMaxLength(10).IsRequired();
        foreach (var p in new[] { "UnitPrice", "BaseAmount", "AgencyAmount", "GajAmount", "DebitAmount", "CreditAmount" }) b.Property(p).HasColumnType("decimal(18,2)").IsRequired();
        foreach (var p in new[] { "AgencyPercent", "GajPercent" }) b.Property(p).HasColumnType("decimal(5,2)").IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.HasIndex(x => x.ExternalInputId).IsUnique();
        b.HasIndex(x => new { x.AgencyId, x.YearId, x.PersianExecutionDate });
    }
}
