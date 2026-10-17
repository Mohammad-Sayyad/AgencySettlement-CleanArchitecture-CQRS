using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.SingleLessonDto
{
    public class CombinedSettlementDetailsDto
    {
        public SettlementDetailsDto Exam { get; set; } = new();

        public SingleLessonReportDto SingleLesson { get; set; } = new();
    }
}
