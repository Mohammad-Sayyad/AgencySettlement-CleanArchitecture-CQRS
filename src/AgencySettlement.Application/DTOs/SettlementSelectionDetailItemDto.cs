using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class SettlementSelectionDetailItemDto
    {
        public long SettlementId { get; init; }

        public long SettlementItemId { get; init; }

        public int PackageId { get; init; }
        public string PackageName { get; init; } = string.Empty;

        public int ExamModeId { get; init; }
        public string ExamModeName { get; init; } = string.Empty;

        public string AgencyName { get; init; } = string.Empty;
        public string YearName { get; init; } = string.Empty;

        public int YearId { get; init; }

        public int EducationalLevelId { get; init; }
        public string EducationalLevelName { get; init; } = string.Empty;

        public int StageTypeId { get; set; }
        public string StageTypeName { get; set; } = string.Empty;

        public int RegistrationPlanId { get; init; }
        public string RegistrationPlanName { get; init; } = string.Empty;

        public int StudyFieldId { get; init; }
        public string StudyFieldName { get; init; } = string.Empty;

        public string PersianExecutionDate { get; init; } = string.Empty;

        public int CandidateCount { get; init; }
        public int FreeCandidateCount { get; init; }
        public int PaidCandidateCount { get; init; }

        public decimal UnitPrice { get; init; }
        public decimal BaseAmount { get; init; }

        public decimal DiscountPercent { get; init; }
        public decimal DiscountAmount { get; init; }

        public decimal AgencyPercent { get; init; }
        public decimal AgencyAmount { get; init; }
        public decimal GajAmount { get; init; }
        public decimal StudentAmount { get; init; }

        public decimal DebitAmount { get; init; }
        public decimal CreditAmount { get; init; }

        public decimal TotalAmount { get; init; }

        public string Title { get; set; } = string.Empty;
    }
}
