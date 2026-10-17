using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.SingleLessonDto;
using MediatR;

namespace AgencySettlement.Application.Features.Queries.GetSettlementDetailsQuery
{
    public sealed class GetSettlementDetailsQueryHandler
      : IRequestHandler<GetSettlementDetailsQuery, CombinedSettlementDetailsDto>
    {
        private readonly ISettlementReportRepository _settlementReportRepository;

        public GetSettlementDetailsQueryHandler(
            ISettlementReportRepository settlementReportRepository)
        {
            _settlementReportRepository = settlementReportRepository;
        }

        public async Task<CombinedSettlementDetailsDto> Handle(
            GetSettlementDetailsQuery request,
            CancellationToken cancellationToken)
        {
            return await _settlementReportRepository.GetCombinedSettlementDetailsAsync(
                request.AgencyId,
                request.YearId,
                request.PersianExecutionDate,
                cancellationToken);
        }
    }
}