using AgencySettlement.Application.Abstractions.Persistence.ComboBoxRepository;
using AgencySettlement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgencySettlement.Application.Settlements.Queries.GetSettlementFiltersLookupQuery.GetSettlementLookupDtoQuery;

namespace AgencySettlement.Application.Settlements.Queries.GetSettlementFiltersLookupQuery
{
    public sealed class GetAgenciesQueryHandler
     : IRequestHandler<
         GetAgenciesQuery,
         IReadOnlyList<AgencyLookupDto>>
    {
        private readonly ISettlementLookupRepository _repository;

        public GetAgenciesQueryHandler(
            ISettlementLookupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AgencyLookupDto>> Handle(
            GetAgenciesQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetAgenciesAsync(
                cancellationToken);
        }
    }


    public sealed class GetYearsQueryHandler
        : IRequestHandler<
            GetYearsQuery,
            IReadOnlyList<YearLookupDto>>
    {
        private readonly ISettlementLookupRepository _repository;

        public GetYearsQueryHandler(
            ISettlementLookupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<YearLookupDto>> Handle(
            GetYearsQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetYearsAsync(
                cancellationToken);
        }
    }


    public sealed class GetExamDatesQueryHandler
        : IRequestHandler<
            GetExamDatesQuery,
            IReadOnlyList<ExamDateLookupDto>>
    {
        private readonly ISettlementLookupRepository _repository;

        public GetExamDatesQueryHandler(
            ISettlementLookupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ExamDateLookupDto>> Handle(
            GetExamDatesQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetExamDatesAsync(
                cancellationToken);
        }
    }
    public sealed class GetSecondExamDatesQueryHandler
        : IRequestHandler<
            GetSecondExamDatesQuery,
            IReadOnlyList<SecondExamDateLookupDto>>
    {
        private readonly ISettlementLookupRepository _repository;

        public GetSecondExamDatesQueryHandler(
            ISettlementLookupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<SecondExamDateLookupDto>> Handle(
            GetSecondExamDatesQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetSecondExamDatesAsync(
                cancellationToken);
        }
    }
}
