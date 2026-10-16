using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations.SingleLesson;

public sealed class SingleLessonSettlementHistoryConfiguration : IEntityTypeConfiguration<SingleLessonSettlementHistory>
{
    public void Configure(EntityTypeBuilder<SingleLessonSettlementHistory> b)
    {
        b.ToTable("SingleLessonSettlementHistories");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Action).HasMaxLength(50).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        foreach (var p in new[] { "TotalDebit", "TotalCredit", "Balance", "TotalAgencyAmount", "TotalGajAmount" }) b.Property(p).HasColumnType("decimal(18,2)").IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.HasIndex(x => x.SettlementId);
    }
}
