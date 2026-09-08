using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class SettlementDebt
    {
        public long Id { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Agency Agency { get; set; } = null!;
    }
}
