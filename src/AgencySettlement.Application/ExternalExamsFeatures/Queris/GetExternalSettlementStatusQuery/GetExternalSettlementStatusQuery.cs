using AgencySettlement.Application.DTOs.ExternalDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Queris.GetExternalSettlementStatusQuery
{
    public sealed record GetExternalSettlementStatusQuery(
     int AgencyId,
     int YearId,
     string PersianExecutionDate)
     : IRequest<ExternalSettlementStatusDto>;
}
