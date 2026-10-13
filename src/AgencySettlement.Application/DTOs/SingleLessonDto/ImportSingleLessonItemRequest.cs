using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.SingleLessonDto
{
    public sealed class ImportSingleLessonItemRequest
    {
        public long CandidateExamId { get; init; }

        public int CourseId { get; init; }

        public int PackageId { get; init; }

        public int EducationalLevelId { get; init; }

        public int ExamModeId { get; init; }

        public int StudyFieldId { get; init; }

        public int RegistrationPlanId { get; init; }
    }
}
