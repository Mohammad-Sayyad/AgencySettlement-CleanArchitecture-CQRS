using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Common
{
    public sealed class SettlementRequest
    {
        public int YearId { get; init; }

        public string PersianExecutionDate { get; init; } = string.Empty;

        public int AgencyId { get; init; }
    }
}
