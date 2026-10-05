using AgencySettlement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface ISettlementFactorReportRepository
    {
        Task<IReadOnlyList<SettlementFactorReportDto>> GetAsync(
            int yearId,
            string persianExecutionDate,
            int? agencyId, int examModeId,
            CancellationToken cancellationToken);
    }
}
