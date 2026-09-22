using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class SettlementFirstOrder
    {
        public long Id { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalDebitGaj { get; set; }

        public decimal TotalCreditGaj { get; set; }

        public decimal BalanceGaj { get; set; }

        public decimal ContractFloorAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<SettlementFirstOrderItem> Items { get; set; }
            = new List<SettlementFirstOrderItem>();
    }
}
