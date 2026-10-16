using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations.SingleLesson;

public sealed class SingleLessonPriceConfiguration : IEntityTypeConfiguration<SingleLessonPrice>
{
    public void Configure(EntityTypeBuilder<SingleLessonPrice> b)
    {
        b.ToTable("SingleLessonPrices");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
        b.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.HasIndex(x => new { x.YearId, x.CourseId, x.StudyFieldId, x.EducationalLevelId, x.PackageId, x.IsActive });
    }
}
