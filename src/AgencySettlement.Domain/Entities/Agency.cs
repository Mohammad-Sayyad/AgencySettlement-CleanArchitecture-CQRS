using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class Agency
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public int DetailCode {  get; set; }

        public int StateId { get; set; }

        public int RegionId { get; set; }
        public int FreeQuotaCount { get; set; }
        public State State { get; set; } = null!;

        public Region Region { get; set; } = null!;
    }
}
