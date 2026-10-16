namespace AgencySettlement.Domain.Entities.SingleLesson;

public sealed class SingleLessonSettlementItem
{
    public long Id { get; set; }
    public long SettlementId { get; set; }
    public long ExternalInputId { get; set; }
    public int AgencyId { get; set; }
    public int CourseId { get; set; }
    public int YearId { get; set; }
    public string PersianExecutionDate { get; set; } = string.Empty;
    public int PackageId { get; set; }
    public int EducationalLevelId { get; set; }
    public int ExamModeId { get; set; }
    public int StudyFieldId { get; set; }
    public int RegistrationPlanId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal AgencyPercent { get; set; }
    public decimal GajPercent { get; set; }
    public decimal AgencyAmount { get; set; }
    public decimal GajAmount { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public SingleLessonSettlement? Settlement { get; set; }
}
