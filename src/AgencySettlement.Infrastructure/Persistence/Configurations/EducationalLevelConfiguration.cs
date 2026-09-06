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
    internal sealed class EducationalLevelConfiguration
     : IEntityTypeConfiguration<EducationalLevel>
    {
        public void Configure(
            EntityTypeBuilder<EducationalLevel> builder)
        {
            builder.ToTable("EducationalLevels");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne(x => x.StageType)
                .WithMany()
                .HasForeignKey(x => x.StageTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
