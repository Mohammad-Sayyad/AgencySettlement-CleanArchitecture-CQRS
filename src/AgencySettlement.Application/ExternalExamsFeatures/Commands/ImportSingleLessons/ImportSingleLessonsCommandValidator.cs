using AgencySettlement.Application.DTOs.SingleLessonDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons
{
    public sealed class ImportSingleLessonsCommandValidator
    {
        public void Validate(ImportSingleLessonRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.AgencyId <= 0)
                throw new ArgumentException("AgencyId is required.");

            if (request.YearId <= 0)
                throw new ArgumentException("YearId is required.");

            if (string.IsNullOrWhiteSpace(request.PersianExecutionDate))
                throw new ArgumentException("PersianExecutionDate is required.");

            if (request.Items is null || request.Items.Count == 0)
                throw new ArgumentException("At least one item is required.");

            foreach (var item in request.Items)
            {
                if (item.CandidateExamId <= 0)
                    throw new ArgumentException("CandidateExamId must be greater than zero.");

                if (item.CourseId <= 0)
                    throw new ArgumentException("CourseId must be greater than zero.");

                if (item.PackageId <= 0)
                    throw new ArgumentException("PackageId must be greater than zero.");

                if (item.EducationalLevelId <= 0)
                    throw new ArgumentException("EducationalLevelId must be greater than zero.");

                if (item.ExamModeId < 0)
                    throw new ArgumentException("ExamModeId cannot be negative.");

                if (item.StudyFieldId <= 0)
                    throw new ArgumentException("StudyFieldId must be greater than zero.");

                if (item.RegistrationPlanId <= 0)
                    throw new ArgumentException("RegistrationPlanId must be greater than zero.");
            }
        }
    }
}
