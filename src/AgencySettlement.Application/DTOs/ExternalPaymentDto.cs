using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public class ExternalPaymentDto
    {
        public long Id { get; set; }

        public long SettlementId { get; set; }

        public decimal Amount { get; set; }

        public DateTime ReceivedAt { get; set; }
    }
}
