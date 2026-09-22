using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.ExternalDtos
{
    public sealed class SettlementOrderReportItemDto
    {
        public int EducationalLevelId { get; set; }
        public int StudyFieldId { get; set; }
        public int ExamModeId { get; set; }

        public int CandidateCount { get; set; }

        public decimal Amount { get; set; }
    }
}
