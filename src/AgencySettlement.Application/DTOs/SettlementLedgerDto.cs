using AgencySettlement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class SettlementLedgerDto
    {
        public long SettlementId { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = null!;

        public int TotalBookletCount { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        public SettlementStatus Status { get; set; }
    }
}
