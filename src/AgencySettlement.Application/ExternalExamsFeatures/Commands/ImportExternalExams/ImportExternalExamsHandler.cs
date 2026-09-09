using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.ExternalExams.Commands.ImportExternalExams;
using AgencySettlement.Domain.Entities;
using MediatR;

namespace AgencySettlement.Application.ExternalExams.Commands;

public sealed class ImportExamRecordsCommandHandler
    : IRequestHandler<ImportExamRecordsCommand, ImportExamResult>
{
    private readonly IExternalExamRecordRepository _repository;
    private readonly ImportExamRecordsCommandValidator _validator;

    public ImportExamRecordsCommandHandler(
        IExternalExamRecordRepository repository,
        ImportExamRecordsCommandValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<ImportExamResult> Handle(
    ImportExamRecordsCommand request,
    CancellationToken cancellationToken)
    {
        _validator.Validate(request);

        var record = request.Record;

        var totalReceived = record.Items.Count;

        var duplicateInRequestCount =
            record.Items
                .GroupBy(x => x.CandidateExamId)
                .Sum(g => g.Count() - 1);

        var items = record.Items
            .GroupBy(x => x.CandidateExamId)
            .Select(x => x.First())
            .ToList();

        var candidateExamIds = items
            .Select(x => x.CandidateExamId)
            .ToList();

        var existingRecords =
            await _repository.GetByCandidateExamIdsAsync(
                candidateExamIds,
                cancellationToken);

        var existingByCandidateExamId =
            existingRecords.ToDictionary(
                x => x.CandidateExamId);

        var newRecords = new List<ExternalExamRecord>();

        foreach (var item in items)
        {
            if (existingByCandidateExamId.TryGetValue(
                    item.CandidateExamId,
                    out var existing))
            {
                existing.PackageId = item.PackageId;
                existing.EducationalLevelId = item.EducationalLevelId;
                existing.ExamModeId = item.ExamModeId;
                existing.StudyFieldId = item.StudyFieldId;
                existing.RegistrationPlanId = item.RegistrationPlanId;
                existing.AgencyId = record.AgencyId;
                existing.PersianExecutionDate =
                    record.PersianExecutionDate;
                existing.YearId = record.YearId;
            }
            else
            {
                newRecords.Add(new ExternalExamRecord
                {
                    CandidateExamId = item.CandidateExamId,
                    PackageId = item.PackageId,
                    EducationalLevelId = item.EducationalLevelId,
                    ExamModeId = item.ExamModeId,
                    StudyFieldId = item.StudyFieldId,
                    RegistrationPlanId = item.RegistrationPlanId,
                    AgencyId = record.AgencyId,
                    PersianExecutionDate =
                        record.PersianExecutionDate,
                    YearId = record.YearId
                });
            }
        }

        if (newRecords.Count > 0)
        {
            await _repository.AddRangeAsync(
                newRecords,
                cancellationToken);
        }

        await _repository.SaveChangesAsync(
            cancellationToken);

        var duplicateRecords =
            existingRecords.Count + duplicateInRequestCount;

        return new ImportExamResult
        {
            TotalReceived = totalReceived,
            NewRecords = newRecords.Count,
            DuplicateRecords = duplicateRecords
        };
    }

    //public async Task<int> Handle(
    //ImportExamRecordsCommand request,
    //CancellationToken cancellationToken)
    //{
    //    _validator.Validate(request);

    //    var record = request.Record;

    //    var items = record.Items
    //        .GroupBy(x => x.CandidateExamId)
    //        .Select(x => x.First())
    //        .ToList();

    //    var candidateExamIds = items
    //        .Select(x => x.CandidateExamId)
    //        .ToList();

    //    var existingRecords =
    //        await _repository.GetByCandidateExamIdsAsync(
    //            candidateExamIds,
    //            cancellationToken);

    //    var existingByCandidateExamId =
    //        existingRecords.ToDictionary(
    //            x => x.CandidateExamId);

    //    var newRecords = new List<ExternalExamRecord>();

    //    foreach (var item in items)
    //    {
    //        if (existingByCandidateExamId.TryGetValue(
    //                item.CandidateExamId,
    //                out var existing))
    //        {
    //            existing.PackageId = item.PackageId;
    //            existing.EducationalLevelId = item.EducationalLevelId;
    //            existing.ExamModeId = item.ExamModeId;
    //            existing.StudyFieldId = item.StudyFieldId;
    //            existing.RegistrationPlanId = item.RegistrationPlanId;
    //            existing.AgencyId = record.AgencyId;
    //            existing.PersianExecutionDate = record.PersianExecutionDate;
    //            existing.YearId = record.YearId;
    //        }
    //        else
    //        {
    //            newRecords.Add(new ExternalExamRecord
    //            {
    //                CandidateExamId = item.CandidateExamId,
    //                PackageId = item.PackageId,
    //                EducationalLevelId = item.EducationalLevelId,
    //                ExamModeId = item.ExamModeId,
    //                StudyFieldId = item.StudyFieldId,
    //                RegistrationPlanId = item.RegistrationPlanId,
    //                AgencyId = record.AgencyId,
    //                PersianExecutionDate = record.PersianExecutionDate,
    //                YearId = record.YearId
    //            });
    //        }
    //    }

    //    if (newRecords.Count > 0)
    //    {
    //        await _repository.AddRangeAsync(
    //            newRecords,
    //            cancellationToken);
    //    }

    //    await _repository.SaveChangesAsync(
    //        cancellationToken);

    //    return items.Count;
    //}

    //public async Task<int> Handle(
    //    ImportExamRecordsCommand request,
    //    CancellationToken cancellationToken)
    //{
    //    _validator.Validate(request);

    //    var record = request.Record;

    //    var candidateExamIds = record.Items
    //        .Select(x => x.CandidateExamId)
    //        .Distinct()
    //        .ToList();

    //    var existingRecords =
    //        await _repository.GetByCandidateExamIdsAsync(
    //            candidateExamIds,
    //            cancellationToken);

    //    var existingRecordsByCandidateExamId =
    //        existingRecords.ToDictionary(
    //            x => x.CandidateExamId);

    //    var newRecords = new List<ExternalExamRecord>();

    //    foreach (var item in record.Items)
    //    {
    //        if (existingRecordsByCandidateExamId.TryGetValue(
    //                item.CandidateExamId,
    //                out var existing))
    //        {
    //            existing.PackageId = item.PackageId;
    //            existing.EducationalLevelId = item.EducationalLevelId;
    //            existing.ExamModeId = item.ExamModeId;
    //            existing.StudyFieldId = item.StudyFieldId;
    //            existing.RegistrationPlanId = item.RegistrationPlanId;

    //            existing.AgencyId = record.AgencyId;
    //            existing.PersianExecutionDate =
    //                record.PersianExecutionDate;
    //            existing.YearId = record.YearId;
    //        }
    //        else
    //        {
    //            newRecords.Add(new ExternalExamRecord
    //            {
    //                CandidateExamId = item.CandidateExamId,
    //                PackageId = item.PackageId,
    //                EducationalLevelId = item.EducationalLevelId,
    //                ExamModeId = item.ExamModeId,
    //                StudyFieldId = item.StudyFieldId,
    //                RegistrationPlanId = item.RegistrationPlanId,

    //                AgencyId = record.AgencyId,
    //                PersianExecutionDate =
    //                    record.PersianExecutionDate,
    //                YearId = record.YearId
    //            });
    //        }
    //    }

    //    if (newRecords.Count > 0)
    //    {
    //        await _repository.AddRangeAsync(
    //            newRecords,
    //            cancellationToken);
    //    }

    //    await _repository.SaveChangesAsync(
    //        cancellationToken);

    //    return candidateExamIds.Count;
    //}
}