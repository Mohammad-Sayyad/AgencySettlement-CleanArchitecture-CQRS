using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Settlements.Queries.DebtQuery
{
    public sealed class GetDebtQueryHandler
     : IRequestHandler<GetDebtQuery, GetDebtResponse>
    {
        private readonly ISettlementDebtRepository _repository;

        public GetDebtQueryHandler(
            ISettlementDebtRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDebtResponse> Handle(
            GetDebtQuery request,
            CancellationToken cancellationToken)
        {
            var settlement =
                await _repository.GetDebtAsync(
                    request.AgencyId,
                    request.YearId,
                    request.PersianExecutionDate,
                    cancellationToken);

            if (settlement is null)
            {
                return new GetDebtResponse
                {
                    AgencyId = request.AgencyId,
                    YearId = request.YearId,
                    PersianExecutionDate =
                        request.PersianExecutionDate,
                    DebitAmount = 0
                };
            }

            return new GetDebtResponse
            {
                AgencyId = settlement.AgencyId,
                YearId = settlement.YearId,
                PersianExecutionDate =
                    settlement.PersianExecutionDate,
                DebitAmount = settlement.TotalDebit
            };
        }
    }
}
