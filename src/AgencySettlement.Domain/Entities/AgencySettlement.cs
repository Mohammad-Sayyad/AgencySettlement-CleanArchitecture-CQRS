using AgencySettlement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class AgencySettlement
    {
        public long SettlementId { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = null!;

        public int TotalBookletCount { get; set; }

        public decimal AgencyDebitAmount { get; set; }

        public decimal AgencyCreditAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        public SettlementStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
