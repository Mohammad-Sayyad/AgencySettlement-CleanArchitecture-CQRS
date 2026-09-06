namespace AgencySettlement.Domain.Entities;

public class Price
{
    public long Id { get; set; }

    public int PackageId { get; set; }

    public int EducationalLevelId { get; set; }

    public int ExamModeId { get; set; }

    public int RegistrationPlanId { get; set; }

    public int YearId { get; set; }

    public decimal Amount { get; set; }

    public string PersianExecutionDate { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}