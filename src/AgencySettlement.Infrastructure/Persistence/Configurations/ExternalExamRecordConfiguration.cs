using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgencySettlement.Infrastructure.Persistence.Configurations;

public sealed class ExternalExamRecordConfiguration
    : IEntityTypeConfiguration<ExternalExamRecord>
{
    public void Configure(
        EntityTypeBuilder<ExternalExamRecord> builder)
    {
        builder.ToTable("ExternalExamRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PersianExecutionDate)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.PersianReceivedDate)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(x => x.CandidateExamId)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.AgencyId,
            x.YearId,
            x.PersianExecutionDate
        });
    }
}
