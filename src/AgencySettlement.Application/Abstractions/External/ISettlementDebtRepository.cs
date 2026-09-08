using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.External
{
    public interface ISettlementDebtRepository
    {
        Task<Settlement?> GetDebtAsync(
            int agencyId,
            int yearId,
            string persianExecutionDate,
            CancellationToken cancellationToken);
    }
}
