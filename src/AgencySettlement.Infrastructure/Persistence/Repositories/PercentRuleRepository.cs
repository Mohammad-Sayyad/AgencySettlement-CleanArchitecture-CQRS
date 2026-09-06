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
    public sealed class PercentRuleRepository
    : IPercentRuleRepository
    {
        private readonly AgencySettlementDbContext _db;

        public PercentRuleRepository(
            AgencySettlementDbContext db)
        {
            _db = db;
        }

        public async Task<Percent?> GetAsync(
      int agencyId,
      int examModeId,
      CancellationToken cancellationToken)
        {
            return await _db.Percents
                .AsNoTracking()
                .Where(x =>
                    x.AgencyId == agencyId &&
                    x.ExamModeId == examModeId  &&
                    x.IsActive)
                .OrderByDescending(x => x.PersianExecutionDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
