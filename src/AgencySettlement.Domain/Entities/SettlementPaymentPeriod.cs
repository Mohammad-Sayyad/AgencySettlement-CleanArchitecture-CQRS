using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public sealed class SettlementPaymentPeriod
    {
        public long Id { get; set; }

        public long SettlementId { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        public string PaymentStartDate { get; set; } = string.Empty;

        public string PaymentDeadlineDate { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
