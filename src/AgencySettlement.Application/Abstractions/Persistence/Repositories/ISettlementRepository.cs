using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface ISettlementRepository
    {
        Task AddAsync(
            Settlement settlement,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);

        Task<List<SettlementItem>> GetSettlementItemsAsync(
        long settlementId,
        CancellationToken cancellationToken);
    }
}
