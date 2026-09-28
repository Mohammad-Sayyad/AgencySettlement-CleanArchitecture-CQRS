using AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand
{
    public sealed record CalculateMonthlySettlementsCommand(
    int YearId,
    int Month)
    : IRequest<MonthlySettlementBatchResult>;
}
