public class SettlementItemResultDto
{
    public int PackageId { get; set; }

    public int EducationalLevelId { get; set; }

    public int? StudyFieldId { get; set; }

    public int ExamModeId { get; set; }

    public int RegistrationPlanId { get; set; }

    public int YearId { get; set; }

    public int CandidateCount { get; set; }

    public int FreeCandidateCount { get; set; }

    public int PaidCandidateCount { get; set; }

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

    public string PersianExecutionDate { get; set; } = string.Empty;
}