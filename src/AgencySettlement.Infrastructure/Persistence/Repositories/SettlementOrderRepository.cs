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
    public sealed class SettlementOrderRepository
    : ISettlementOrderRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementOrderRepository(AgencySettlementDbContext context)
        {
            _context = context;
        }

        public Task<SettlementOrder?> GetByAgencyYearAndOrderAsync(
            int agencyId,
            int yearId,
            int registrationOrder,
            CancellationToken cancellationToken)
        {
            return _context.SettlementOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x =>
                        x.AgencyId == agencyId &&
                        x.YearId == yearId &&
                        x.RegistrationOrder == registrationOrder,
                    cancellationToken);
        }

        public async Task AddAsync(
            SettlementOrder entity,
            CancellationToken cancellationToken)
        {
            await _context.SettlementOrders.AddAsync(
                entity,
                cancellationToken);
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<SettlementOrder>> GetForMonthlyCalculationAsync(
    int yearId,
    List<int> agencyIds,
    CancellationToken cancellationToken)
        {
            if (agencyIds.Count == 0)
            {
                return [];
            }

            return await _context.SettlementOrders
                .Include(x => x.Items)
                .Where(x =>
                    x.YearId == yearId &&
                    agencyIds.Contains(x.AgencyId))
                .ToListAsync(cancellationToken);
        }
    }
}
