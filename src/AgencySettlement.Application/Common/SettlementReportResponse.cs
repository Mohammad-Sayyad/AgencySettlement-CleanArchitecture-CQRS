using AgencySettlement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Common
{
    public sealed class SettlementReportResponse
    {
        public List<SettlementReportData> Items { get; set; } = [];

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public decimal TotalDebitAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal TotalBalance { get; set; }
        public int TotalBookletCount { get; set; }
    }
}
