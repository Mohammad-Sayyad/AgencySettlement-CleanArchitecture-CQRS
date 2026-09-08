using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface IAgencySettlementRepository
    {
        Task<decimal?> GetDebitAmountAsync(
            int yearId,
            string persianExecutionDate,
            int agencyId,
            CancellationToken cancellationToken);
    }
}
