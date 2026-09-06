using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface IAgencyRepository
    {
        Task<Agency?> GetByIdAsync(
            int agencyId,
            CancellationToken cancellationToken);

        Task<int> ConsumeFreeQuotaAsync(
            int agencyId,
            int requestedCount,
            CancellationToken cancellationToken);
        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
