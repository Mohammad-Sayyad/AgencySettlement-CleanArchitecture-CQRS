using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs.ExternalDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Queris.GetExternalSettlementStatusQuery
{
    public sealed class GetExternalSettlementStatusQueryHandler
    : IRequestHandler<
        GetExternalSettlementStatusQuery,
        ExternalSettlementStatusDto>
    {
        private readonly IExternalSettlementStatusRepository _repository;

        public GetExternalSettlementStatusQueryHandler(
            IExternalSettlementStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExternalSettlementStatusDto> Handle(
            GetExternalSettlementStatusQuery request,
            CancellationToken cancellationToken)
        {
            if (request.AgencyId <= 0)
                throw new ArgumentException(
                    "AgencyId نامعتبر است.");

            if (request.YearId <= 0)
                throw new ArgumentException(
                    "YearId نامعتبر است.");

            if (string.IsNullOrWhiteSpace(
                    request.PersianExecutionDate))
                throw new ArgumentException(
                    "PersianExecutionDate الزامی است.");

            var result =
                await _repository.GetExternalSettlementStatusAsync(
                    request.AgencyId,
                    request.YearId,
                    request.PersianExecutionDate,
                    cancellationToken);

            if (result == null)
                throw new InvalidOperationException(
                    "Settlement یا بازه پرداخت موردنظر پیدا نشد.");

            return result;
        }
    }
}
