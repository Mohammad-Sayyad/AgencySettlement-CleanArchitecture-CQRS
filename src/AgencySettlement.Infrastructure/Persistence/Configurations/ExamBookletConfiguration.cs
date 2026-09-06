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
    internal sealed class ExamBookletConfiguration
     : IEntityTypeConfiguration<ExamBooklet>
    {
        public void Configure(
            EntityTypeBuilder<ExamBooklet> builder)
        {
            builder.ToTable("ExamBooklets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.PackageId,
                x.EducationalLevelId,
                x.StudyFieldId
            });
        }
    }
}
