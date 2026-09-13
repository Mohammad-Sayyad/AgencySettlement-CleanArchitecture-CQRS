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
     string FromDateId,
     string ToDateId,
     int PageNumber,
     int PageSize)
     : IRequest<SettlementReportResponse>;
}
