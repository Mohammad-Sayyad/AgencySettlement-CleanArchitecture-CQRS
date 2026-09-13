using AgencySettlement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.ComboBoxRepository
{
    public interface ISettlementLookupRepository
    {
        Task<IReadOnlyList<AgencyLookupDto>> GetAgenciesAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<YearLookupDto>> GetYearsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<ExamDateLookupDto>> GetExamDatesAsync(
            CancellationToken cancellationToken);
        Task<IReadOnlyList<SecondExamDateLookupDto>> GetSecondExamDatesAsync(
            CancellationToken cancellationToken);

   //     Task<IReadOnlyList<ExamDateLookupDto>> GetExamDatesAsync(
   //string fromDate,
   //string toDate,
   //CancellationToken cancellationToken);
    }
}
