using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.External
{
    public interface IExternalExamRecordRepository
    {
        Task<List<ExternalExamRecord>> GetByCandidateExamIdsAsync(
       List<long> candidateExamIds,
       CancellationToken cancellationToken);

        Task AddRangeAsync(
            List<ExternalExamRecord> records,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);

        Task<List<ExternalExamRecord>> GetByAgencyAndYearAsync(
    int agencyId,
    int yearId,
    string persianExecutionDate,
    CancellationToken cancellationToken);
    }
}

