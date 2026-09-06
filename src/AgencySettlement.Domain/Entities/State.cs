using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Entities
{
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Region> Regions { get; set; } = new List<Region>();
    }
}
