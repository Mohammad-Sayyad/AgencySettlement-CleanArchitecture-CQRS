using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{

    public sealed class ExternalExamRecordHistory
    {
        public long Id { get; set; }

        public long ExternalExamRecordId { get; set; }

        public long CandidateExamId { get; set; }

        public int PackageId { get; set; }

        public int EducationalLevelId { get; set; }

        public int ExamModeId { get; set; }

        public int StudyFieldId { get; set; }

        public int RegistrationPlanId { get; set; }

        public int AgencyId { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;

        public int YearId { get; set; }

        public int RegistrationOrder { get; set; }

        public string PersianReceivedDate { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }
    }
}
