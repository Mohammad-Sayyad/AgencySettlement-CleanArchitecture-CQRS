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
    internal sealed class RegistrationPlanConfiguration
    : IEntityTypeConfiguration<RegistrationPlan>
    {
        public void Configure(
            EntityTypeBuilder<RegistrationPlan> builder)
        {
            builder.ToTable("RegistrationPlans");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();
        }
    }
}
