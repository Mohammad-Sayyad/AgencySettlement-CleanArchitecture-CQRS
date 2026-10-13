using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities.SingleLesson
{
    public sealed class SingleLessonExternalInput
    {
        public long Id { get; set; }

        public long CandidateExamId { get; set; }

        public int CourseId { get; set; }
        public int PackageId { get; set; }
        public int EducationalLevelId { get; set; }
        public int ExamModeId { get; set; }
        public int StudyFieldId { get; set; }
        public int RegistrationPlanId { get; set; }

        public int AgencyId { get; set; }
        public int YearId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        public int? RegistrationOrder { get; set; }

        public string? PersianReceivedDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
