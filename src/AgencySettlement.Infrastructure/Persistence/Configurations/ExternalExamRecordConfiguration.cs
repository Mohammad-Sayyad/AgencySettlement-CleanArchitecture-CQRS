using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations;

internal sealed class ExternalExamRecordConfiguration : IEntityTypeConfiguration<ExternalExamRecord>
{
    public void Configure(EntityTypeBuilder<ExternalExamRecord> builder)
    {
        builder.ToTable("ExternalExamRecords", table =>
        {
            table.HasCheckConstraint("CK_ExternalExamRecords_CandidateExamId", "[CandidateExamId] > 0");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CandidateExamId)
            .IsUnique()
            .HasDatabaseName("UX_ExternalExamRecords_CandidateExamId");

        builder.HasIndex(x => new { x.AgencyId, x.YearId, x.PersianExecutionDate })
            .HasDatabaseName("IX_ExternalExamRecords_Agency_Year_Date");

        builder.HasIndex(x => new { x.RegistrationPlanId, x.YearId, x.PersianExecutionDate })
            .HasDatabaseName("IX_ExternalExamRecords_ExamType_Year_Date");
    }
}
