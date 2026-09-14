using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementSelectionDetailsQuery
{
    public sealed class GetSettlementSelectionDetailsQueryHandler
    : IRequestHandler<
        GetSettlementSelectionDetailsQuery,
        SettlementSelectionDetailsDto>
    {
        private readonly ISettlementReportRepository _repository;

        public GetSettlementSelectionDetailsQueryHandler(
            ISettlementReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<SettlementSelectionDetailsDto> Handle(
            GetSettlementSelectionDetailsQuery request,
            CancellationToken cancellationToken)
        {
            if (request.AgencyId <= 0)
                throw new ArgumentException("AgencyId نامعتبر است.");

            if (request.SettlementIds is null ||
                request.SettlementIds.Length == 0)
            {
                throw new ArgumentException(
                    "حداقل یک Settlement باید انتخاب شود.");
            }

            var settlementIds = request.SettlementIds
                .Where(x => x > 0)
                .Distinct()
                .ToArray();

            if (settlementIds.Length == 0)
                throw new ArgumentException(
                    "Settlement انتخاب‌شده معتبر نیست.");

            return await _repository.GetSettlementSelectionDetailsAsync(
                request.AgencyId,
                settlementIds,
                cancellationToken);
        }
    }
}
