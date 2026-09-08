using AgencySettlement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface ISettlementLedgerRepository
    {
        Task<IReadOnlyList<SettlementLedgerDto>> GetLedgerAsync(
            int yearId,
            string persianExecutionDate,
            int? agencyId,
            CancellationToken cancellationToken);
    }
}
