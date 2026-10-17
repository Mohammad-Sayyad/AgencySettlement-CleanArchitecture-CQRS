using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities.SingleLesson
{
    public sealed class SingleLessons
    {
        public int Id { get; set; }

        public int YearId { get; set; }

        public int CourseId { get; set; }

        public int StudyFieldId { get; set; }

        public int EducationalLevelId { get; set; }

        public int PackageId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
