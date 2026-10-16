using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Domain.Entities.SingleLesson;
using MediatR;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons;

public sealed class ImportSingleLessonsCommandHandler : IRequestHandler<ImportSingleLessonsCommand, ImportExamResult>
{
    private readonly ISingleLessonExternalInputRepository _repository;
    private readonly ImportSingleLessonsCommandValidator _validator;
    private readonly ISingleLessonSettlementCoordinator _settlements;
    private readonly IUnitOfWork _unitOfWork;

    public ImportSingleLessonsCommandHandler(ISingleLessonExternalInputRepository repository,
        ImportSingleLessonsCommandValidator validator, ISingleLessonSettlementCoordinator settlements, IUnitOfWork unitOfWork)
    { _repository = repository; _validator = validator; _settlements = settlements; _unitOfWork = unitOfWork; }

    public async Task<ImportExamResult> Handle(ImportSingleLessonsCommand request, CancellationToken cancellationToken)
    {
        _validator.Validate(request.Record);
        var record = request.Record;
        var totalReceived = record.Items.Count;
        var duplicateInRequestCount = record.Items.GroupBy(x => x.CandidateExamId).Sum(g => g.Count() - 1);
        var items = record.Items.GroupBy(x => x.CandidateExamId).Select(g => g.First()).ToList();
        var ids = items.Select(x => x.CandidateExamId).ToList();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var existingRecords = await _repository.GetByCandidateExamIdsAsync(ids, cancellationToken);
            var existingById = existingRecords.ToDictionary(x => x.CandidateExamId);
            var affected = new HashSet<SingleLessonSettlementGroup>();
            var newRecords = new List<SingleLessonExternalInput>();
            var histories = new List<SingleLessonExternalInputHistory>();
            foreach (var item in items)
            {
                if (existingById.TryGetValue(item.CandidateExamId, out var existing))
                {
                    affected.Add(new(existing.AgencyId, existing.YearId, existing.PersianExecutionDate));
                    var changes = new List<string>();
                    AddChange(changes, nameof(existing.CourseId), existing.CourseId, item.CourseId);
                    AddChange(changes, nameof(existing.PackageId), existing.PackageId, item.PackageId);
                    AddChange(changes, nameof(existing.EducationalLevelId), existing.EducationalLevelId, item.EducationalLevelId);
                    AddChange(changes, nameof(existing.ExamModeId), existing.ExamModeId, item.ExamModeId);
                    AddChange(changes, nameof(existing.StudyFieldId), existing.StudyFieldId, item.StudyFieldId);
                    AddChange(changes, nameof(existing.RegistrationPlanId), existing.RegistrationPlanId, item.RegistrationPlanId);
                    AddChange(changes, nameof(existing.AgencyId), existing.AgencyId, record.AgencyId);
                    AddChange(changes, nameof(existing.YearId), existing.YearId, record.YearId);
                    AddChange(changes, nameof(existing.PersianExecutionDate), existing.PersianExecutionDate, record.PersianExecutionDate);
                    if (changes.Count > 0) histories.Add(new SingleLessonExternalInputHistory { ExternalInputId=existing.Id, CandidateExamId=existing.CandidateExamId, CourseId=existing.CourseId, PackageId=existing.PackageId, EducationalLevelId=existing.EducationalLevelId, ExamModeId=existing.ExamModeId, StudyFieldId=existing.StudyFieldId, RegistrationPlanId=existing.RegistrationPlanId, AgencyId=existing.AgencyId, YearId=existing.YearId, PersianExecutionDate=existing.PersianExecutionDate, RegistrationOrder=existing.RegistrationOrder, PersianReceivedDate=existing.PersianReceivedDate, Description=string.Join(" ", changes), ChangedAt=DateTime.UtcNow });
                    existing.CourseId=item.CourseId; existing.PackageId=item.PackageId; existing.EducationalLevelId=item.EducationalLevelId; existing.ExamModeId=item.ExamModeId; existing.StudyFieldId=item.StudyFieldId; existing.RegistrationPlanId=item.RegistrationPlanId; existing.AgencyId=record.AgencyId; existing.YearId=record.YearId; existing.PersianExecutionDate=record.PersianExecutionDate;
                    affected.Add(new(existing.AgencyId, existing.YearId, existing.PersianExecutionDate));
                }
                else
                {
                    var added = new SingleLessonExternalInput { CandidateExamId=item.CandidateExamId, CourseId=item.CourseId, PackageId=item.PackageId, EducationalLevelId=item.EducationalLevelId, ExamModeId=item.ExamModeId, StudyFieldId=item.StudyFieldId, RegistrationPlanId=item.RegistrationPlanId, AgencyId=record.AgencyId, YearId=record.YearId, PersianExecutionDate=record.PersianExecutionDate, CreatedAt=DateTime.UtcNow };
                    newRecords.Add(added);
                    affected.Add(new(record.AgencyId, record.YearId, record.PersianExecutionDate));
                }
            }
            if (histories.Count > 0) await _repository.AddHistoryRangeAsync(histories, cancellationToken);
            if (newRecords.Count > 0) await _repository.AddRangeAsync(newRecords, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            await _settlements.RecalculateAsync(affected, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ImportExamResult { TotalReceived=totalReceived, NewRecords=newRecords.Count, DuplicateRecords=existingRecords.Count + duplicateInRequestCount };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static void AddChange<T>(List<string> changes, string field, T oldValue, T newValue)
    { if (!EqualityComparer<T>.Default.Equals(oldValue, newValue)) changes.Add($"{field} از {oldValue} به {newValue} تغییر کرد."); }
}
