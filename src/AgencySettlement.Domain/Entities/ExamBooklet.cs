using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class ExamBooklet
    {
        public long Id { get; set; }

        public int PackageId { get; set; }

        public int EducationalLevelId { get; set; }

        public int StudyFieldId { get; set; }

        public string Name { get; set; } = null!;
    }
}
