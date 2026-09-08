using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.External
{
    public interface IAgencySettlementRepository
    {
        Task<decimal?> GetDebtAmountAsync(
            int yearId,
            string persianExecutionDate,
            int agencyId,
            CancellationToken cancellationToken);
    }
}
