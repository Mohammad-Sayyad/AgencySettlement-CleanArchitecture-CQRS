using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class SettlementDetailItemDto
    {
        public long SettlementItemId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;

        public int ExamModeId { get; set; }
        public string ExamModeName { get; set; } = string.Empty;

        public int StageTypeId { get; set; }
        public string StageTypeName { get; set; } = string.Empty;

        public int EducationalLevelId { get; set; }
        public string EducationalLevelName { get; set; } = string.Empty;

        public int StudyFieldId { get; set; }
        public string StudyFieldName { get; set; } = string.Empty;

        public int CandidateCount { get; set; }
        public int FreeCandidateCount { get; set; }
        public int PaidCandidateCount { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

      

        public decimal DiscountPercent { get; set; }


        public decimal AgencyPercent { get; set; }
        public decimal AgencyAmount { get; set; }

        public decimal CreditAmount { get; set; }

  
    }
}
