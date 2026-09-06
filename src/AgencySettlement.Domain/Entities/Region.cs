using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int StateId { get; set; }
        public State State { get; set; } = null!;

        public ICollection<Agency> Agencies { get; set; } = new List<Agency>();
    }
}
