using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementFactorReportQuery
{

    public sealed record GetSettlementFactorReportQuery(
       int YearId,
       string PersianExecutionDate,
       int? AgencyId , int examModeId
   ) : IRequest<IReadOnlyList<SettlementFactorReportDto>>;
}
