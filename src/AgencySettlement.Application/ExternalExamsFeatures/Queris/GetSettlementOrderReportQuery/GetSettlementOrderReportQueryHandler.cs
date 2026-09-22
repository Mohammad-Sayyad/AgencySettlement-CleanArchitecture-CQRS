using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs.ExternalDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Queris.GetSettlementOrderReportQuery
{
    public sealed class GetSettlementOrderReportQueryHandler
    : IRequestHandler<
        GetSettlementOrderReportQuery,
        SettlementOrderReportDto?>
    {
        private readonly IExternalSettlementStatusRepository _repository;

        public GetSettlementOrderReportQueryHandler(
            IExternalSettlementStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<SettlementOrderReportDto?> Handle(
            GetSettlementOrderReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetSettlementOrderReportAsync(
                request.AgencyId,
                request.YearId,
                request.PersianExecutionDate,
                request.RegistrationOrder,
                cancellationToken);
        }
    }
}
