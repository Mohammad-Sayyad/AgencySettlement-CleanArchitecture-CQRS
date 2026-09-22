using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Configurations
{
    public sealed class ExternalExamRecordHistoryConfiguration
    : IEntityTypeConfiguration<ExternalExamRecordHistory>
    {
        public void Configure(
            EntityTypeBuilder<ExternalExamRecordHistory> builder)
        {
            builder.ToTable("ExternalExamRecordHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PersianExecutionDate)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.PersianReceivedDate)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.ChangedAt)
                .IsRequired();

            builder.HasIndex(x => x.ExternalExamRecordId);

            builder.HasIndex(x => x.CandidateExamId);
        }
    }
}
