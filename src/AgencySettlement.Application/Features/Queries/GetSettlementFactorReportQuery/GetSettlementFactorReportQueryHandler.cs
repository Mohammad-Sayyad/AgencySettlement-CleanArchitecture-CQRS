using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementFactorReportQuery
{

    public sealed class GetSettlementFactorReportQueryHandler
        : IRequestHandler<
            GetSettlementFactorReportQuery,
            IReadOnlyList<SettlementFactorReportDto>>
    {
        private readonly ISettlementFactorReportRepository _repository;

        public GetSettlementFactorReportQueryHandler(
            ISettlementFactorReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<SettlementFactorReportDto>> Handle(
            GetSettlementFactorReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetAsync(
                request.YearId,
                request.PersianExecutionDate,
                request.AgencyId,
                request.examModeId,
                cancellationToken);
        }
    }
}
