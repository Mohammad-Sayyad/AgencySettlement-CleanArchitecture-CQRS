using AgencySettlement.Application.DTOs.ExternalDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.External
{

    public interface IExternalSettlementStatusRepository
    {
        Task<ExternalSettlementStatusDto?> GetExternalSettlementStatusAsync(
       int agencyId,
       int yearId,
       string persianExecutionDate,
       CancellationToken cancellationToken);
    }
}
