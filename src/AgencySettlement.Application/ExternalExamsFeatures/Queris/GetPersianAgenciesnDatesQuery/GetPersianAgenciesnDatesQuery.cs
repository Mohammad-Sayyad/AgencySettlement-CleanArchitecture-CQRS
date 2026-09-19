using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Queris.GetSettlementDatesQuery
{

    public sealed record GetPersianAgenciesnDatesQuery(
        int AgencyId,
        int YearId) : IRequest<List<string>>;
}
