using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using MediatR;

namespace AgencySettlement.Application.Features.Queries.GetSettlementDetailsQuery
{
    public sealed class GetSettlementDetailsQueryHandler
        : IRequestHandler<GetSettlementDetailsQuery, SettlementDetailsDto>
    {
        private readonly ISettlementReportRepository _repository;

        public GetSettlementDetailsQueryHandler(
            ISettlementReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<SettlementDetailsDto> Handle(
            GetSettlementDetailsQuery request,
            CancellationToken cancellationToken)
        {
            //if (request.AgencyId <= 0)
            //    throw new ArgumentException("AgencyId نامعتبر است.");

            if (request.YearId <= 0)
                throw new ArgumentException("YearId نامعتبر است.");

            if (string.IsNullOrWhiteSpace(request.PersianExecutionDate))
                throw new ArgumentException("تاریخ اجرا الزامی است.");

            return await _repository.GetSettlementDetailsAsync(
                request.AgencyId,
                request.YearId,
                request.PersianExecutionDate,
                cancellationToken);
        }
    }
}