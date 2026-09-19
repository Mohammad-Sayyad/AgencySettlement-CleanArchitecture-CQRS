using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.External
{
    public interface ISettlementPaymentRepository
    {
        Task<decimal> GetPaymentAmountAsync(
            int agencyId,
            int yearId,
            string persianExecutionDate,
            CancellationToken cancellationToken);

        Task AddSettlementPaymentAsync(
    SettlementPayment payment,
    CancellationToken cancellationToken);

        Task<Settlement?> GetSettlementByIdAsync(
            long settlementId,
            CancellationToken cancellationToken);

        IQueryable<SettlementPayment> GetSettlementPayments();

        Task<Settlement?> GetSettlementByAgencyAndDateAsync(
    int agencyId,
    int yearId,
    string persianExecutionDate,
    CancellationToken cancellationToken);
    }
}
