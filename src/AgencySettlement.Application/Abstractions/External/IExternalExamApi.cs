namespace AgencySettlement.Application.Abstractions.External;

public sealed class ExternalExamItem
{
    public long CandidateExamId { get; set; }

    public int PackageId { get; set; }

    public int? EducationalLevelId { get; set; }

    public int ExamModeId { get; set; }

    public int ExamTypeId { get; set; }

    public int StudyFieldId { get; set; }
    public int RegistrationPlanId { get; set; }
}
public sealed class ExternalExamGroup
{
    public int AgencyId { get; set; }

    public int YearId { get; set; }

    public DateTime PersianExecutionDate { get; set; }

    public List<ExternalExamItem> Items { get; set; } = [];
}
public interface IExternalExamApi
{
    Task<IReadOnlyList<ExternalExamGroup>> GetExamsAsync(
    CancellationToken cancellationToken = default);
}
