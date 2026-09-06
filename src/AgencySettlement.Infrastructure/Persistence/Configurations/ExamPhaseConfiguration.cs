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
    internal sealed class ExamPhaseConfiguration
    : IEntityTypeConfiguration<ExamPhase>
    {
        public void Configure(
            EntityTypeBuilder<ExamPhase> builder)
        {
            builder.ToTable("ExamPhases");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();
        }
    }
}
