using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Models
{
    public sealed class MonthlySettlementBatchResult
    {
        public int YearId { get; init; }

        public int Month { get; init; }

        public int AgencyCount { get; init; }

        public int OrderCount { get; init; }

        public int OrderItemCount { get; init; }

        public int SettlementCount { get; init; }

        public int HistoryCount { get; init; }

        public decimal TotalDebit { get; init; }

        public decimal TotalCredit { get; init; }

        public decimal TotalDebitGaj { get; init; }

        public decimal TotalCreditGaj { get; init; }
    }
}
