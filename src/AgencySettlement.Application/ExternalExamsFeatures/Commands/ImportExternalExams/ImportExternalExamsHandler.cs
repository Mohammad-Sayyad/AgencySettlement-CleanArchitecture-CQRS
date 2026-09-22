using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Common;
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

        const int registrationOrder = 2;

        var receivedDate = "05/07/24";

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

        var newRecords =
            new List<ExternalExamRecord>();

        var histories =
            new List<ExternalExamRecordHistory>();

        foreach (var item in items)
        {
            if (existingByCandidateExamId.TryGetValue(
                    item.CandidateExamId,
                    out var existing))
            {
                var changes = new List<string>();

                if (existing.PackageId != item.PackageId)
                {
                    changes.Add(
                        $"PackageId از {existing.PackageId} به {item.PackageId} تغییر کرد.");
                }

                if (existing.EducationalLevelId != item.EducationalLevelId)
                {
                    changes.Add(
                        $"EducationalLevelId از {existing.EducationalLevelId} به {item.EducationalLevelId} تغییر کرد.");
                }

                if (existing.ExamModeId != item.ExamModeId)
                {
                    changes.Add(
                        $"ExamModeId از {existing.ExamModeId} به {item.ExamModeId} تغییر کرد.");
                }

                if (existing.StudyFieldId != item.StudyFieldId)
                {
                    changes.Add(
                        $"StudyFieldId از {existing.StudyFieldId} به {item.StudyFieldId} تغییر کرد.");
                }

                if (existing.RegistrationPlanId != item.RegistrationPlanId)
                {
                    changes.Add(
                        $"RegistrationPlanId از {existing.RegistrationPlanId} به {item.RegistrationPlanId} تغییر کرد.");
                }

                if (existing.AgencyId != record.AgencyId)
                {
                    changes.Add(
                        $"AgencyId از {existing.AgencyId} به {record.AgencyId} تغییر کرد.");
                }

                if (existing.PersianExecutionDate !=
                    record.PersianExecutionDate)
                {
                    changes.Add(
                        $"PersianExecutionDate از {existing.PersianExecutionDate} به {record.PersianExecutionDate} تغییر کرد.");
                }

                if (existing.YearId != record.YearId)
                {
                    changes.Add(
                        $"YearId از {existing.YearId} به {record.YearId} تغییر کرد.");
                }

                if (existing.RegistrationOrder !=
                    registrationOrder)
                {
                    changes.Add(
                        $"RegistrationOrder از {existing.RegistrationOrder} به {registrationOrder} تغییر کرد.");
                }

                if (changes.Count > 0)
                {
                    histories.Add(new ExternalExamRecordHistory
                    {
                        ExternalExamRecordId = existing.Id,
                        CandidateExamId = existing.CandidateExamId,
                        PackageId = existing.PackageId,
                        EducationalLevelId = existing.EducationalLevelId,
                        ExamModeId = existing.ExamModeId,
                        StudyFieldId = existing.StudyFieldId,
                        RegistrationPlanId = existing.RegistrationPlanId,
                        AgencyId = existing.AgencyId,
                        PersianExecutionDate =
                            existing.PersianExecutionDate,
                        YearId = existing.YearId,
                        RegistrationOrder =
                            existing.RegistrationOrder,
                        PersianReceivedDate =
                            existing.PersianReceivedDate,
                        Description =
                            string.Join(" ", changes),
                        ChangedAt = DateTime.Now
                    });
                }

                existing.PackageId = item.PackageId;
                existing.EducationalLevelId =
                    item.EducationalLevelId;
                existing.ExamModeId = item.ExamModeId;
                existing.StudyFieldId = item.StudyFieldId;
                existing.RegistrationPlanId =
                    item.RegistrationPlanId;
                existing.AgencyId = record.AgencyId;
                existing.PersianExecutionDate =
                    record.PersianExecutionDate;
                existing.YearId = record.YearId;
                existing.RegistrationOrder =
                    registrationOrder;
                existing.PersianReceivedDate =
                    receivedDate;
            }
            else
            {
                newRecords.Add(new ExternalExamRecord
                {
                    CandidateExamId =
                        item.CandidateExamId,

                    PackageId =
                        item.PackageId,

                    EducationalLevelId =
                        item.EducationalLevelId,

                    ExamModeId =
                        item.ExamModeId,

                    StudyFieldId =
                        item.StudyFieldId,

                    RegistrationPlanId =
                        item.RegistrationPlanId,

                    AgencyId =
                        record.AgencyId,

                    PersianExecutionDate =
                        record.PersianExecutionDate,

                    YearId =
                        record.YearId,

                    RegistrationOrder =
                        registrationOrder,

                    PersianReceivedDate =
                        receivedDate
                });
            }
        }

        if (histories.Count > 0)
        {
            await _repository.AddHistoryRangeAsync(
                histories,
                cancellationToken);
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
            existingRecords.Count +
            duplicateInRequestCount;

        return new ImportExamResult
        {
            TotalReceived = totalReceived,
            NewRecords = newRecords.Count,
            DuplicateRecords = duplicateRecords
        };
    }
}