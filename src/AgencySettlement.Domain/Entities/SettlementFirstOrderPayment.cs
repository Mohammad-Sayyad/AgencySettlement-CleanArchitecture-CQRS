using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class SettlementFirstOrderPayment
    {
        public long Id { get; set; }

        public long SettlementFirstOrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentDate { get; set; } = string.Empty;

        public string TrackingNumber { get; set; } = string.Empty;

        public string PaymentReference { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public SettlementFirstOrder SettlementFirstOrder { get; set; }
            = null!;
    }
}
