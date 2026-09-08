using AgencySettlement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Settlements.Queries.PaymentQuery
{
    public sealed record GetPaymentQuery(
    int AgencyId,
    int YearId,
    string PersianExecutionDate
) : IRequest<GetPaymentResponse>;
}
