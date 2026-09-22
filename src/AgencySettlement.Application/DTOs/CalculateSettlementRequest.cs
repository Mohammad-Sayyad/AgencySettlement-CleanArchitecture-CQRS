using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class CalculateSettlementRequest
    {
        public int AgencyId { get; set; }

        public int YearId { get; set; }
        public string PersianExecutionDate { get; set; } = string.Empty;
        public int RegistrationOrder { get; set; }
    }
}
