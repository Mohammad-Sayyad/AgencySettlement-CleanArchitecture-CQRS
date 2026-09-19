using AgencySettlement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.ExternalDtos
{
    public sealed class ExternalSettlementStatusDto
    {
        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public long SettlmentId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        public string PaymentDeadline { get; set; } = string.Empty;

        public int Status { get; set; }

        public string StatusTitle { get; set; } = string.Empty;

        public decimal DebtAmount { get; set; }
        public decimal Balance { get; set; }
        public SettlementPaymentStatus PaymentStatus { get; set; }
    }
}
