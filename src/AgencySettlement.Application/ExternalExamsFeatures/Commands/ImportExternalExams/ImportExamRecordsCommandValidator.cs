using AgencySettlement.Application.DTOs;

namespace AgencySettlement.Application.ExternalExams.Commands.ImportExternalExams;

public sealed class ImportExamRecordsCommandValidator
{
    public void Validate(
        ImportExamRecordsCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var record = command.Record;

        if (record is null)
            throw new ArgumentException(
                "اطلاعات آزمون الزامی است.");

        if (record.AgencyId <= 0)
            throw new ArgumentException(
                "AgencyId نامعتبر است.");

        if (record.YearId <= 0)
            throw new ArgumentException(
                "YearId نامعتبر است.");

        if (string.IsNullOrWhiteSpace(record.PersianExecutionDate))
            throw new ArgumentException(
                "PersianExecutionDate الزامی است.");

        if (record.Items is null || record.Items.Count == 0)
            throw new ArgumentException(
                "لیست آیتم‌های آزمون نمی‌تواند خالی باشد.");

        foreach (var item in record.Items)
        {
            if (item.CandidateExamId <= 0)
                throw new ArgumentException(
                    "CandidateExamId نامعتبر است.");

            if (item.PackageId <= 0)
                throw new ArgumentException(
                    "PackageId نامعتبر است.");

            if (item.EducationalLevelId <= 0)
                throw new ArgumentException(
                    "EducationalLevelId نامعتبر است.");

            //if (item.ExamModeId <= 0)
            //    throw new ArgumentException(
            //        "ExamModeId نامعتبر است.");

            if (item.StudyFieldId <= 0)
                throw new ArgumentException(
                    "StudyFieldId نامعتبر است.");

            if (item.RegistrationPlanId <= 0)
                throw new ArgumentException(
                    "RegistrationPlanId نامعتبر است.");
        }
    }
}