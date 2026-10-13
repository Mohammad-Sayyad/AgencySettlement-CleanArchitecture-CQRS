using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Domain.Entities.SingleLesson;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons
{
    public sealed class ImportSingleLessonsCommandHandler
    : IRequestHandler<ImportSingleLessonsCommand, ImportExamResult>
    {
        private readonly ISingleLessonExternalInputRepository _repository;
        private readonly ImportSingleLessonsCommandValidator _validator;

        public ImportSingleLessonsCommandHandler(
            ISingleLessonExternalInputRepository repository,
            ImportSingleLessonsCommandValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<ImportExamResult> Handle(
            ImportSingleLessonsCommand request,
            CancellationToken cancellationToken)
        {
            _validator.Validate(request.Record);
            // مطابق منطق نمونه ارسالی شما
            const int registrationOrder = 2;
            const string receivedDate = "05/07/23";

            var record = request.Record;
            var totalReceived = record.Items.Count;

            var duplicateInRequestCount = record.Items
                .GroupBy(x => x.CandidateExamId)
                .Sum(g => g.Count() - 1);

            var items = record.Items
                .GroupBy(x => x.CandidateExamId)
                .Select(g => g.First())
                .ToList();

            var candidateExamIds = items
                .Select(x => x.CandidateExamId)
                .ToList();

            var existingRecords =
                await _repository.GetByCandidateExamIdsAsync(
                    candidateExamIds,
                    cancellationToken);

            var existingByCandidateExamId = existingRecords
                .ToDictionary(x => x.CandidateExamId);

            var newRecords = new List<SingleLessonExternalInput>();
            var histories = new List<SingleLessonExternalInputHistory>();

            foreach (var item in items)
            {
                if (existingByCandidateExamId.TryGetValue(
                        item.CandidateExamId,
                        out var existing))
                {
                    var changes = new List<string>();

                    if (existing.CourseId != item.CourseId)
                        changes.Add(
                            $"CourseId از {existing.CourseId} به {item.CourseId} تغییر کرد.");

                    if (existing.PackageId != item.PackageId)
                        changes.Add(
                            $"PackageId از {existing.PackageId} به {item.PackageId} تغییر کرد.");

                    if (existing.EducationalLevelId != item.EducationalLevelId)
                        changes.Add(
                            $"EducationalLevelId از {existing.EducationalLevelId} به {item.EducationalLevelId} تغییر کرد.");

                    if (existing.ExamModeId != item.ExamModeId)
                        changes.Add(
                            $"ExamModeId از {existing.ExamModeId} به {item.ExamModeId} تغییر کرد.");

                    if (existing.StudyFieldId != item.StudyFieldId)
                        changes.Add(
                            $"StudyFieldId از {existing.StudyFieldId} به {item.StudyFieldId} تغییر کرد.");

                    if (existing.RegistrationPlanId != item.RegistrationPlanId)
                        changes.Add(
                            $"RegistrationPlanId از {existing.RegistrationPlanId} به {item.RegistrationPlanId} تغییر کرد.");

                    if (existing.AgencyId != record.AgencyId)
                        changes.Add(
                            $"AgencyId از {existing.AgencyId} به {record.AgencyId} تغییر کرد.");

                    if (existing.YearId != record.YearId)
                        changes.Add(
                            $"YearId از {existing.YearId} به {record.YearId} تغییر کرد.");

                    if (existing.PersianExecutionDate != record.PersianExecutionDate)
                        changes.Add(
                            $"PersianExecutionDate از {existing.PersianExecutionDate} به {record.PersianExecutionDate} تغییر کرد.");

                    if (existing.RegistrationOrder != registrationOrder)
                        changes.Add(
                            $"RegistrationOrder از {existing.RegistrationOrder} به {registrationOrder} تغییر کرد.");

                    if (existing.PersianReceivedDate != receivedDate)
                        changes.Add(
                            $"PersianReceivedDate از {existing.PersianReceivedDate} به {receivedDate} تغییر کرد.");

                    if (changes.Count > 0)
                    {
                        histories.Add(new SingleLessonExternalInputHistory
                        {
                            ExternalInputId = existing.Id,
                            CandidateExamId = existing.CandidateExamId,
                            CourseId = existing.CourseId,
                            PackageId = existing.PackageId,
                            EducationalLevelId = existing.EducationalLevelId,
                            ExamModeId = existing.ExamModeId,
                            StudyFieldId = existing.StudyFieldId,
                            RegistrationPlanId = existing.RegistrationPlanId,
                            AgencyId = existing.AgencyId,
                            YearId = existing.YearId,
                            PersianExecutionDate = existing.PersianExecutionDate,
                            RegistrationOrder = existing.RegistrationOrder,
                            PersianReceivedDate = existing.PersianReceivedDate,
                            Description = string.Join(" ", changes),
                            ChangedAt = DateTime.Now
                        });
                    }

                    existing.CourseId = item.CourseId;
                    existing.PackageId = item.PackageId;
                    existing.EducationalLevelId = item.EducationalLevelId;
                    existing.ExamModeId = item.ExamModeId;
                    existing.StudyFieldId = item.StudyFieldId;
                    existing.RegistrationPlanId = item.RegistrationPlanId;
                    existing.AgencyId = record.AgencyId;
                    existing.YearId = record.YearId;
                    existing.PersianExecutionDate = record.PersianExecutionDate;
                    existing.RegistrationOrder = registrationOrder;
                    existing.PersianReceivedDate = receivedDate;
                }
                else
                {
                    newRecords.Add(new SingleLessonExternalInput
                    {
                        CandidateExamId = item.CandidateExamId,
                        CourseId = item.CourseId,
                        PackageId = item.PackageId,
                        EducationalLevelId = item.EducationalLevelId,
                        ExamModeId = item.ExamModeId,
                        StudyFieldId = item.StudyFieldId,
                        RegistrationPlanId = item.RegistrationPlanId,
                        AgencyId = record.AgencyId,
                        YearId = record.YearId,
                        PersianExecutionDate = record.PersianExecutionDate,
                        RegistrationOrder = registrationOrder,
                        PersianReceivedDate = receivedDate
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

            await _repository.SaveChangesAsync(cancellationToken);

            return new ImportExamResult
            {
                TotalReceived = totalReceived,
                NewRecords = newRecords.Count,
                DuplicateRecords = existingRecords.Count + duplicateInRequestCount
            };
        }
    }
}
