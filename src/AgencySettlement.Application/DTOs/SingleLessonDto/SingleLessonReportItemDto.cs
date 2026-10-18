using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.SingleLessonDto
{
    public class SingleLessonReportItemDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int EducationalLevelId { get; set; }
        public string EducationalLevelName { get; set; } = string.Empty;

        public int StudyFieldId { get; set; }
        public string StudyFieldName { get; set; } = string.Empty;

        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;

        public int ExamModeId { get; set; }
        public string ExamModeName { get; set; } = string.Empty;

        public int RegistrationPlanId { get; set; }
        public string RegistrationPlanName { get; set; } = string.Empty;

        public int SingleLessonCandidateCount { get; set; }
        public int SingleLessonQuantity { get; set; }

        public decimal SingleLessonTotalDebit { get; set; }
        public decimal SingleLessonTotalCredit { get; set; }
        public decimal SingleLessonBalance { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal BaseAmount { get; set; }

        public decimal AgencyPercent { get; set; }
        public decimal GajPercent { get; set; }

        public decimal AgencyAmount { get; set; }
        public decimal GajAmount { get; set; }

        public int StageTypeId { get; set; }
        public string StageTypeName { get; set; } = string.Empty;
    }
}
