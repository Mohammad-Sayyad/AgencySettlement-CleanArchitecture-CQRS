using AgencySettlement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class SettlementReportData
    {
        public int AgencyId { get; init; }

        public int DetailCode { get; init; }
           
        public string AgencyName { get; init; }
            = string.Empty;

        public int YearId { get; init; }

        public string PersianExecutionDate { get; init; }
            = string.Empty;

        public int BookletCount { get; init; }

        public decimal DebitAmount { get; init; }

        public decimal CreditAmount { get; init; }

        public decimal Balance { get; init; }

        public SettlementStatus Status { get; set; }

        // اگر Settlement فیلدهای دیگری دارد
        // اینجا هم اضافه می‌کنیم.
    }
}
