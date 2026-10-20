using AgencySettlement.Application.DTOs.ReportsDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementPaymentsReportQuery
{
    public sealed record GetSettlementPaymentsReportQuery(
    int? YearId,
    string? PersianExecutionDate,
    int? AgencyId
) : IRequest<List<SettlementPaymentReportDto>>;
}
