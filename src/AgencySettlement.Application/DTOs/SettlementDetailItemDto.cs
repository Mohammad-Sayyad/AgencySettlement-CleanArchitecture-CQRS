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
        public long SettlementId { get; set; }
        public decimal AgencyPercent { get; init; }

        public decimal GajPercent { get; init; }
        public string Title { get; set; } = string.Empty;
        public string YearName { get; set; } = string.Empty;
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public int ExamModeId { get; set; }
        public string ExamModeName { get; set; } = string.Empty;
        public string RegistrationPlanName { get; set; } = string.Empty;
        public string PersianExecutionDate { get; set; } = string.Empty;
        public int RegistrationPlanId { get; set; }
        public int StageTypeId { get; set; }
        public string StageTypeName { get; set; } = string.Empty;
        public int DetailCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EducationalLevelId { get; set; }
        public string EducationalLevelName { get; set; } = string.Empty;

        public int StudyFieldId { get; set; }
        public string StudyFieldName { get; set; } = string.Empty;

        public decimal StudentAmount { get; set; }
        public decimal StudentPercent { get; set; }

        public decimal AgencyAmount { get; set; }

        public decimal GajAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public int YearId { get; set; }
        public int CandidateCount { get; set; }
        public int FreeCandidateCount { get; set; }
        public int PaidCandidateCount { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

       public int AgencyId { get; set; } 

        public decimal DiscountPercent { get; set; }


    

        public decimal CreditAmount { get; set; }

        public decimal TotalBaseAmount { get; set; }

        public decimal TotalStudentAmount { get; set; }

        public decimal TotalAgencyAmount { get; set; }

        public decimal TotalGajAmount { get; set; }

        public decimal RemainingAmount { get; set; }


    }
}
