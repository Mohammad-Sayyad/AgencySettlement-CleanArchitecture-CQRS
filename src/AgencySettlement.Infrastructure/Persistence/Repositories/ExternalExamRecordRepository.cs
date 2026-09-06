using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Domain.Entities;

using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    public sealed class ExternalExamRecordRepository
    : IExternalExamRecordRepository
    {
        private readonly AgencySettlementDbContext _db;

        public ExternalExamRecordRepository(
            AgencySettlementDbContext db)
        {
            _db = db;
        }

        public async Task<List<ExternalExamRecord>> GetByCandidateExamIdsAsync(
       List<long> candidateExamIds,
       CancellationToken cancellationToken)
        {
            return await _db.ExternalExamRecords
                .Where(x => candidateExamIds.Contains(x.CandidateExamId))
                .ToListAsync(cancellationToken);
        }

        public async Task AddRangeAsync(
            List<ExternalExamRecord> records,
            CancellationToken cancellationToken)
        {
            await _db.ExternalExamRecords.AddRangeAsync(
                records,
                cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ExternalExamRecord>>
    GetByAgencyAndYearAsync(
        int agencyId,
        int yearId,
        CancellationToken cancellationToken)
        {
            return await _db.ExternalExamRecords
                .Where(x =>
                    x.AgencyId == agencyId &&
                    x.YearId == yearId)
                .ToListAsync(cancellationToken);
        }
    }
}
