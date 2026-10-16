using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations.SingleLesson;

public sealed class SingleLessonPercentConfiguration : IEntityTypeConfiguration<SingleLessonPercent>
{
    public void Configure(EntityTypeBuilder<SingleLessonPercent> b)
    {
        b.ToTable("SingleLessonPercents", t => t.HasCheckConstraint("CK_SingleLessonPercents_Sum100", "[AgencyPercent] >= 0 AND [GajPercent] >= 0 AND [AgencyPercent] + [GajPercent] = 100"));
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.AgencyPercent).HasColumnType("decimal(5,2)").IsRequired();
        b.Property(x => x.GajPercent).HasColumnType("decimal(5,2)").IsRequired();
        b.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.HasIndex(x => new { x.YearId, x.CourseId, x.StudyFieldId, x.EducationalLevelId, x.PackageId, x.AgencyId, x.IsActive });
    }
}
