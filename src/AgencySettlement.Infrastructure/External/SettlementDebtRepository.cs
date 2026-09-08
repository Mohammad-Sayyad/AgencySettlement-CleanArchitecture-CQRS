using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Domain.Entities;
using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.External
{
    public sealed class SettlementDebtRepository
    : ISettlementDebtRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementDebtRepository(
            AgencySettlementDbContext context)
        {
            _context = context;
        }

        public async Task<Settlement?> GetDebtAsync(
            int agencyId,
            int yearId,
            string persianExecutionDate,
            CancellationToken cancellationToken)
        {
            return await _context.Settlements
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.AgencyId == agencyId &&
                        x.YearId == yearId &&
                        x.PersianExecutionDate ==
                            persianExecutionDate,
                    cancellationToken);
        }
    }
}
