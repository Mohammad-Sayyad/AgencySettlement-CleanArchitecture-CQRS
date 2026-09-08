using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Common;
using AgencySettlement.Domain.Entities;
using AgencySettlement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _repository.GetReportAsync(
                request.AgencyId,
                request.YearId,
                request.PersianExecutionDate,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
