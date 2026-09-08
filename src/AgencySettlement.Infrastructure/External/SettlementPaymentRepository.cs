using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.External
{
    public sealed class SettlementPaymentRepository
    : ISettlementPaymentRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementPaymentRepository(
            AgencySettlementDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetPaymentAmountAsync(
            int agencyId,
            int yearId,
            string persianExecutionDate,
            CancellationToken cancellationToken)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(
                    x =>
                        x.AgencyId == agencyId &&
                        x.YearId == yearId &&
                        x.PersianExecutionDate ==
                            persianExecutionDate)
                .SumAsync(
                    x => x.Amount,
                    cancellationToken);
        }
    }
}
