using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.SingleLessonDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementDetailsQuery
{
    //public sealed record GetSettlementDetailsQuery(
    //int AgencyId,
    //int YearId,
    //string PersianExecutionDate)
    //: IRequest<SettlementDetailsDto>;

    public record GetSettlementDetailsQuery(
     int AgencyId,
     int YearId,
     string PersianExecutionDate
 ) : IRequest<CombinedSettlementDetailsDto>;
}
