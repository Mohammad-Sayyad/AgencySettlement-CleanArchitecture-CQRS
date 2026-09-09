using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Settlements.Queries.GetSettlementFiltersLookupQuery
{
    public sealed class GetSettlementLookupDtoQuery
    {
        public sealed record GetAgenciesQuery
    : IRequest<IReadOnlyList<AgencyLookupDto>>;

        public sealed record GetYearsQuery
            : IRequest<IReadOnlyList<YearLookupDto>>;

        public sealed record GetExamDatesQuery
            : IRequest<IReadOnlyList<ExamDateLookupDto>>;
    }
}
