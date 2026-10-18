using AgencySettlement.Domain.Entities.SingleLesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{

    public class SettlementPayment
    {
        public long Id { get; set; }

        public long SettlementId { get; set; }

        public int AgencyId { get; set; }

        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        // مجموع مبلغ پرداختی
        public decimal Amount { get; set; }

        // سهم پرداخت بابت آزمون
        public decimal ExamAmount { get; set; }

        // سهم پرداخت بابت تک‌درس
        public decimal SingleLessonAmount { get; set; }

        // در پرداختی که تک‌درس ندارد، null خواهد بود
        public long? SingleLessonSettlementId { get; set; }

        public string PaymentDate { get; set; } = string.Empty;

        public string TrackingNumber { get; set; } = string.Empty;

        public bool IsAppliedToSettlement { get; set; }

        public Agency Agency { get; set; } = null!;

        public Settlement Settlement { get; set; } = null!;

        public SingleLessonSettlement? SingleLessonSettlement { get; set; }
    }
}
