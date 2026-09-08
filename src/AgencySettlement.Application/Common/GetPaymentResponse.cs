using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Common
{
    public sealed class GetPaymentResponse
    {
        public int AgencyId { get; init; }

        public int YearId { get; init; }

        public string PersianExecutionDate { get; init; }
            = string.Empty;

        public decimal CreditAmount { get; init; }
    }
}
