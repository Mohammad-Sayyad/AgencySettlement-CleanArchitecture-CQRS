using AgencySettlement.Application.Common;
using AgencySettlement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface ISettlementReportRepository
    {
        //Task<SettlementReportResponse> GetReportAsync(
        //   int? agencyId,
        //   int yearId,
        //   string[] persianExecutionDate,
        //   int pageNumber,
        //   int pageSize,
        //   CancellationToken cancellationToken);

        Task<SettlementReportResponse> GetReportAsync(
    int? agencyId,
    int yearId,
    string fromDateId,
    string toDateId,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken);

        //    Task<List<SettlementDetailItemDto>> GetSettlementDetailsAsync(
        //int agencyId,
        //int yearId,
        //string persianExecutionDate,
        //CancellationToken cancellationToken);

        Task<SettlementDetailsDto> GetSettlementDetailsAsync(
    int agencyId,
    int yearId,
    string persianExecutionDate,
    CancellationToken cancellationToken);


        Task<SettlementSelectionDetailsDto> GetSettlementSelectionDetailsAsync(
     int agencyId,
     long[] settlementIds,
     CancellationToken cancellationToken);
    }
}
