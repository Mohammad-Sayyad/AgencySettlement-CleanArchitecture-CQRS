using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    public sealed class SettlementHistoryRepository
    : ISettlementHistoryRepository
    {
        private readonly AgencySettlementDbContext _db;

        public SettlementHistoryRepository(
            AgencySettlementDbContext db)
        {
            _db = db;
        }

        public async Task AddRangeAsync(
    List<SettlementHistory> histories,
    CancellationToken cancellationToken)
        {
            if (histories.Count == 0)
            {
                return;
            }

            await _db.SettlementHistories.AddRangeAsync(
                histories,
                cancellationToken);
        }

        public async Task AddAsync(
            SettlementHistory history,
            CancellationToken cancellationToken)
        {
            await _db.SettlementHistories.AddAsync(
                history,
                cancellationToken);
        }
    }
}
