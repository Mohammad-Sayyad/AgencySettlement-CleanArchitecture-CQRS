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
    }
}
