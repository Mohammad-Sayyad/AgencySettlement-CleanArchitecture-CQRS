using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Domain.Entities.SingleLesson;
using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.External.SingleLesson
{
    public sealed class SingleLessonExternalInputRepository
    : ISingleLessonExternalInputRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SingleLessonExternalInputRepository(
            AgencySettlementDbContext context)
        {
            _context = context;
        }

        public Task<List<SingleLessonExternalInput>> GetByCandidateExamIdsAsync(
            List<long> candidateExamIds,
            CancellationToken cancellationToken)
        {
            return _context.SingleLessonExternalInputs
                .Where(x => candidateExamIds.Contains(x.CandidateExamId))
                .ToListAsync(cancellationToken);
        }

        public async Task AddRangeAsync(
            List<SingleLessonExternalInput> records,
            CancellationToken cancellationToken)
        {
            await _context.SingleLessonExternalInputs
                .AddRangeAsync(records, cancellationToken);
        }

        public async Task AddHistoryRangeAsync(
            List<SingleLessonExternalInputHistory> histories,
            CancellationToken cancellationToken)
        {
            await _context.SingleLessonExternalInputHistories
                .AddRangeAsync(histories, cancellationToken);
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
