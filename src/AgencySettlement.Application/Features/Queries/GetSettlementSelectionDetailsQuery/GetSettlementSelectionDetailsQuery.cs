using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementSelectionDetailsQuery
{
    public sealed record GetSettlementSelectionDetailsQuery(
     int AgencyId,
     long[] SettlementIds
 ) : IRequest<SettlementSelectionDetailsDto>;
}
