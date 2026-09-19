using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Queris.GetSettlementDatesQuery
{
    public sealed class GetPersianAgenciesnDatesQueryHandler
     : IRequestHandler<GetPersianAgenciesnDatesQuery, List<string>>
    {
        private readonly IExternalSettlementStatusRepository _externalSettlementStatusRepository;

        public GetPersianAgenciesnDatesQueryHandler(
            IExternalSettlementStatusRepository externalSettlementStatusRepository)
        {
            _externalSettlementStatusRepository = externalSettlementStatusRepository;
        }

        public async Task<List<string>> Handle(
            GetPersianAgenciesnDatesQuery request,
            CancellationToken cancellationToken)
        {

            //if (request.AgencyId <= 0)
            //    throw new ArgumentException("AgencyId نامعتبر است.");

            //if (request.YearId <= 0)
            //    throw new ArgumentException("YearId نامعتبر است.");

            return await _externalSettlementStatusRepository
                .GetPersianAgenciesnDatesAsync(
                    request.AgencyId,
                    request.YearId,
                    cancellationToken);
        }
    }
}
