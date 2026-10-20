using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs.ReportsDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Queries.GetSettlementPaymentsReportQuery
{
    public sealed class GetSettlementPaymentsReportQueryHandler
    : IRequestHandler<
        GetSettlementPaymentsReportQuery,
        List<SettlementPaymentReportDto>>
    {
        private readonly ISettlementPaymentRepository _repository;

        public GetSettlementPaymentsReportQueryHandler(
            ISettlementPaymentRepository repository)
        {
            _repository = repository;
        }

        public Task<List<SettlementPaymentReportDto>> Handle(
            GetSettlementPaymentsReportQuery request,
            CancellationToken cancellationToken)
        {
            return _repository.GetReportAsync(
                request.YearId,
                request.PersianExecutionDate,
                request.AgencyId,
                cancellationToken);
        }
    }
}