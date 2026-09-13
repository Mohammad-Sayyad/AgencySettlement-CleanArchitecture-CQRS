using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Common;
using MediatR;

namespace AgencySettlement.Application.Settlements.Queries.ReportQuery
{
    public sealed class GetSettlementReportQueryHandler
        : IRequestHandler<
            GetSettlementReportQuery,
            SettlementReportResponse>
    {
        private readonly ISettlementReportRepository _repository;

        public GetSettlementReportQueryHandler(
            ISettlementReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<SettlementReportResponse> Handle(
            GetSettlementReportQuery request,
            CancellationToken cancellationToken)
        {
            if (request.YearId <= 0)
                throw new ArgumentException(
                    "YearId نامعتبر است.");

            if (string.IsNullOrWhiteSpace(request.FromDateId))
                throw new ArgumentException(
                    "FromDate نامعتبر است.");

            if (string.IsNullOrWhiteSpace(request.ToDateId))
                throw new ArgumentException(
                    "ToDate نامعتبر است.");

            return await _repository.GetReportAsync(
                request.AgencyId,
                request.YearId,
                request.FromDateId,
                request.ToDateId,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}