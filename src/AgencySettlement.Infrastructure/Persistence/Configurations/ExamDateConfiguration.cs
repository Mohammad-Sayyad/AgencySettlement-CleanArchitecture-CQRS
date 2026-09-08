using AgencySettlement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Configurations
{
    public sealed class ExamDateConfiguration
    : IEntityTypeConfiguration<ExamDate>
    {
        public void Configure(EntityTypeBuilder<ExamDate> builder)
        {
            builder.ToTable("ExamDates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PersianDate)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
        }
    }
}
