using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations.SingleLesson;

public sealed class SingleLessonSettlementConfiguration : IEntityTypeConfiguration<SingleLessonSettlement>
{
    public void Configure(EntityTypeBuilder<SingleLessonSettlement> b)
    {
        b.ToTable("SingleLessonSettlements");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.PersianExecutionDate).HasMaxLength(10).IsRequired();
        foreach (var p in new[] { "TotalDebit", "TotalCredit", "Balance", "TotalAgencyAmount", "TotalGajAmount" })
            b.Property(p).HasColumnType("decimal(18,2)").IsRequired();
        b.Property(x => x.TotalCourseCount).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.HasIndex(x => new { x.AgencyId, x.YearId, x.PersianExecutionDate });
        b.HasMany(x => x.Items).WithOne(x => x.Settlement).HasForeignKey(x => x.SettlementId).OnDelete(DeleteBehavior.Cascade);
    }
}
