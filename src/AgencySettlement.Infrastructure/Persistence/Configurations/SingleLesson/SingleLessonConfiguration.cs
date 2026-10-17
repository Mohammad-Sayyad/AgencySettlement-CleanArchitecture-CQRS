using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Configurations.SingleLesson
{
    public sealed class SingleLessonConfiguration
    : IEntityTypeConfiguration<SingleLessons>
    {
        public void Configure(EntityTypeBuilder<SingleLessons> builder)
        {
            builder.ToTable("SingleLessons");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.YearId)
                .IsRequired();

            builder.Property(x => x.CourseId)
                .IsRequired();

            builder.Property(x => x.StudyFieldId)
                .IsRequired();

            builder.Property(x => x.EducationalLevelId)
                .IsRequired();

            builder.Property(x => x.PackageId)
                .IsRequired();

            builder.Property(x => x.CourseName)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
