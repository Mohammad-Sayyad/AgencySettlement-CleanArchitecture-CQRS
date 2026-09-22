using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface ISettlementOrderRepository
    {
        Task<SettlementOrder?> GetByAgencyYearAndOrderAsync(
            int agencyId,
            int yearId,
            int registrationOrder,
            CancellationToken cancellationToken);

        Task AddAsync(
            SettlementOrder entity,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
