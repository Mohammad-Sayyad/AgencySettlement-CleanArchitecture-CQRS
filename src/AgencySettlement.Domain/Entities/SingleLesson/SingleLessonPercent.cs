namespace AgencySettlement.Domain.Entities.SingleLesson;

public sealed class SingleLessonPercent
{
    public int Id { get; set; }
    public int YearId { get; set; }
    public int CourseId { get; set; }
    public int StudyFieldId { get; set; }
    public int EducationalLevelId { get; set; }
    public int PackageId { get; set; }
    public int? AgencyId { get; set; }
    public decimal AgencyPercent { get; set; }
    public decimal GajPercent { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
