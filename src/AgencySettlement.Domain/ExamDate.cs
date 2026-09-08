using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain
{
    public sealed class ExamDate
    {
        public int Id { get; set; }
        public string PersianDate { get; set; } = string.Empty;
    }
}
