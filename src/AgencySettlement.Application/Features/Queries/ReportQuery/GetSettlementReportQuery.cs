using AgencySettlement.Application.Common;
using AgencySettlement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Settlements.Queries.ReportQuery
{
    public sealed record GetSettlementReportQuery(
    int? AgencyId,
    int YearId,
    string PersianExecutionDate,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<SettlementReportResponse>;
}
