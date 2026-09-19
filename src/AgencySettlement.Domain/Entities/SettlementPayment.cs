using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class SettlementPayment
    {
        public long Id { get; set; }

        public long SettlementId { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; }
            = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentDate { get; set; }
            = string.Empty;

        public string TrackingNumber { get; set; }
            = string.Empty;

        public string PaymentReference { get; set; }
            = string.Empty;

        public Agency Agency { get; set; } = null!;

        public Settlement Settlement { get; set; } = null!;
    }
}
