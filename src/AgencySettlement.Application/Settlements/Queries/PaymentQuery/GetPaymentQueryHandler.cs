using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Settlements.Queries.PaymentQuery
{
    public sealed class GetPaymentQueryHandler
    : IRequestHandler<GetPaymentQuery, GetPaymentResponse>
    {
        private readonly ISettlementPaymentRepository _repository;

        public GetPaymentQueryHandler(
            ISettlementPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPaymentResponse> Handle(
            GetPaymentQuery request,
            CancellationToken cancellationToken)
        {
            var amount =
                await _repository.GetPaymentAmountAsync(
                    request.AgencyId,
                    request.YearId,
                    request.PersianExecutionDate,
                    cancellationToken);

            return new GetPaymentResponse
            {
                AgencyId = request.AgencyId,
                YearId = request.YearId,
                PersianExecutionDate =
                    request.PersianExecutionDate,
                CreditAmount = amount
            };
        }
    }
}
