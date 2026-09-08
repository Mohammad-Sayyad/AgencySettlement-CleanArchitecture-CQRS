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
    }
}
