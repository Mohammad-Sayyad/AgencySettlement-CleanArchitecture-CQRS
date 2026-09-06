using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{

    public class Settlement
    {
        public long Id { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<SettlementItem> Items { get; set; } = [];
        public decimal TotalDebitGaj { get; set; }
        public decimal TotalCreditGaj { get; set; }
        public string PersianExecutionDate { get; set; } = string.Empty;

    }
}
