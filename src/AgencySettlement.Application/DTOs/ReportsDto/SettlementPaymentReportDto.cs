using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.ReportsDto
{
    public sealed class SettlementPaymentReportDto
    {
        public long PaymentId { get; set; }

        public int AgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public int DetailCode { get; set; }

        public int YearId { get; set; }
        public string? PersianExecutionDate { get; set; }
        public string? PaymentDate { get; set; }
        public string? TrackingNumber { get; set; }

        public decimal ExamAmount { get; set; }
        public decimal SingleLessonAmount { get; set; }
        public decimal TotalPaymentAmount { get; set; }

        public decimal Amount { get; set; }
        public bool IsAppliedToSettlement { get; set; }
    }
}
