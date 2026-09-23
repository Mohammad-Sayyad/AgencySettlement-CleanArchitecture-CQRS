using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation
{
    public sealed record CalculateSettlementCommand(
    CalculateSettlementRequest Request)
    : IRequest<SettlementResultDto>;
}
