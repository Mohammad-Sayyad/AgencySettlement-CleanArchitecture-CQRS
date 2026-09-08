using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class Payment
    {
        public long Id { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; }
            = string.Empty;

        public decimal Amount { get; set; }

        public Agency Agency { get; set; } = null!;
    }
}
