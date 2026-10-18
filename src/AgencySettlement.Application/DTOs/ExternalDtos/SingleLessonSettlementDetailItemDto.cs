namespace AgencySettlement.Application.DTOs.SingleLessonDto;

public sealed class SingleLessonSettlementDetailItemDto
{
    public long Id { get; set; }

    public long SettlementId { get; set; }

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int PackageId { get; set; }

    public string PackageName { get; set; } = string.Empty;

    public int EducationalLevelId { get; set; }

    public string EducationalLevelName { get; set; } = string.Empty;

    public int StudyFieldId { get; set; }

    public string StudyFieldName { get; set; } = string.Empty;

    public int ExamModeId { get; set; }

    public string ExamModeName { get; set; } = string.Empty;

    public int RegistrationPlanId { get; set; }

    public string RegistrationPlanName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal BaseAmount { get; set; }

    public decimal AgencyPercent { get; set; }

    public decimal GajPercent { get; set; }

    public decimal AgencyAmount { get; set; }

    public decimal GajAmount { get; set; }

    public decimal DebitAmount { get; set; }
}