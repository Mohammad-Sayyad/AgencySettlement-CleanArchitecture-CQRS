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
    public sealed class SingleLessonExternalInputConfiguration
    : IEntityTypeConfiguration<SingleLessonExternalInput>
    {
        public void Configure(
            EntityTypeBuilder<SingleLessonExternalInput> builder)
        {
            builder.ToTable("SingleLessonExternalInputs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn();

            builder.Property(x => x.CandidateExamId)
                .IsRequired();

            builder.Property(x => x.CourseId)
                .IsRequired();

            builder.Property(x => x.PackageId)
                .IsRequired();

            builder.Property(x => x.EducationalLevelId)
                .IsRequired();

            builder.Property(x => x.ExamModeId)
                .IsRequired();

            builder.Property(x => x.StudyFieldId)
                .IsRequired();

            builder.Property(x => x.RegistrationPlanId)
                .IsRequired();

            builder.Property(x => x.AgencyId)
                .IsRequired();

            builder.Property(x => x.YearId)
                .IsRequired();

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.RegistrationOrder)
                .IsRequired(false);

            builder.Property(x => x.PersianReceivedDate)
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()")
                .IsRequired();

            builder.HasMany<SingleLessonExternalInputHistory>()
                .WithOne()
                .HasForeignKey(x => x.ExternalInputId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CandidateExamId);
        }
    }
}
