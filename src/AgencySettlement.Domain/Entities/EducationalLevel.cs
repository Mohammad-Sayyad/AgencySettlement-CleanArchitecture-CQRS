using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class EducationalLevel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int StageTypeId { get; set; }
        public StageType StageType { get; set; } = null!;
    }
}
