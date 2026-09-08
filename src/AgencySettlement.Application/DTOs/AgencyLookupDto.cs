using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class AgencyLookupDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DetailCode { get; set; } 

        public int StateId { get; set; }

        public int RegionId { get; set; }
    }
}
