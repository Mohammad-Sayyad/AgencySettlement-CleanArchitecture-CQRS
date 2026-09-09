using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Settlements.Queries
{
    public sealed record GetExamsLookupQuery
    : IRequest<IReadOnlyList<ExamDateLookupDto>>;
}
