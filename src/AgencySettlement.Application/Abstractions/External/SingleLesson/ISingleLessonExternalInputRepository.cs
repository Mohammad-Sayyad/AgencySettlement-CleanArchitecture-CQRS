using AgencySettlement.Domain.Entities.SingleLesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.External.SingleLesson
{
    public interface ISingleLessonExternalInputRepository
    {
        Task<List<SingleLessonExternalInput>> GetByCandidateExamIdsAsync(
            List<long> candidateExamIds,
            CancellationToken cancellationToken);

        Task AddRangeAsync(
            List<SingleLessonExternalInput> records,
            CancellationToken cancellationToken);

        Task AddHistoryRangeAsync(
            List<SingleLessonExternalInputHistory> histories,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
