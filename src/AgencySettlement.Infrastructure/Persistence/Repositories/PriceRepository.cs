using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    public sealed class PriceRepository : IPriceRepository
    {
        private readonly AgencySettlementDbContext _db;

        public PriceRepository(
            AgencySettlementDbContext db)
        {
            _db = db;
        }

        public async Task<Price?> GetAsync(
     int packageId,
     int educationalLevelId,
     int examModeId,
     int registrationPlanId,
     int yearId,
     CancellationToken cancellationToken)
        {
            return await _db.Prices
                .AsNoTracking()
                .Where(x =>
                    x.PackageId == packageId &&
                    x.EducationalLevelId == educationalLevelId &&
                    x.ExamModeId == examModeId &&
                    x.RegistrationPlanId == registrationPlanId &&
                    x.YearId == yearId &&
                    x.IsActive)
                .OrderByDescending(x => x.PersianExecutionDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}