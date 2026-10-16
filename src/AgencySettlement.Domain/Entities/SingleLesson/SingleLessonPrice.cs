namespace AgencySettlement.Domain.Entities.SingleLesson;

public sealed class SingleLessonPrice
{
    public int Id { get; set; }
    public int YearId { get; set; }
    public int CourseId { get; set; }
    public int StudyFieldId { get; set; }
    public int EducationalLevelId { get; set; }
    public int PackageId { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
