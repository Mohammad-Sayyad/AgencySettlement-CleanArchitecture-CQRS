using System;

namespace AgencySettlement.Domain.Entities
{
    public class SettlementItem
    {
        public long Id { get; set; }

        public long SettlementId { get; set; }

        public int AgencyId { get; set; }

        public int PackageId { get; set; }

        public int EducationalLevelId { get; set; }

        public int StudyFieldId { get; set; }

        public decimal ContractFloorAmount { get; set; }

        public int FreeCandidateCount { get; set; }

        public int PaidCandidateCount { get; set; }

        public int ExamModeId { get; set; }

        public int RegistrationPlanId { get; set; }

        public int YearId { get; set; }

        public int CandidateCount { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal BaseAmount { get; set; }

        public decimal AgencyPercent { get; set; }

        public decimal GajPercent { get; set; }

        public decimal StudentPercent { get; set; }

        public decimal AgencyAmount { get; set; }

        public decimal GajAmount { get; set; }

        public decimal StudentAmount { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public int FreeQuotaCount { get; set; }

        public int OneHundredThousandQuotaCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PersianExecutionDate { get; set; } = string.Empty;
    }
}