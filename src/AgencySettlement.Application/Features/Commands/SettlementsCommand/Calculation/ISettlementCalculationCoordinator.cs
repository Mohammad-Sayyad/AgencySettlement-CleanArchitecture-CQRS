using AgencySettlement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation
{
    public interface ISettlementCalculationCoordinator
    {
        Task<SettlementResultDto> CalculateAsync(
            CalculateSettlementRequest request,
            CancellationToken cancellationToken);
    }
}
