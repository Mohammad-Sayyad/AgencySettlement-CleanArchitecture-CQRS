using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.SingleLessonDto
{
    public class SingleLessonReportDto
    {
        public List<SingleLessonReportItemDto> Items { get; set; } = [];

        public decimal SingleLessonTotalDebit { get; set; }

        public decimal SingleLessonTotalCredit { get; set; }

        public decimal SingleLessonBalance { get; set; }

        public int SingleLessonTotalCourseCount { get; set; }

        public int TotalCandidateCount { get; set; }

        public decimal TotalBaseAmount { get; set; }
        public decimal TotalAgencyAmount { get; set; }
        public decimal TotalGajAmount { get; set; }

        public decimal SettlementTotalCredit { get; set; }
        public decimal SettlementBalance { get; set; }
    }
}
