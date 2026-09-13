using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class ExamDateLookupDto
    {
        public int Id { get; set; }
        public string PersianDate { get; set; } = string.Empty;
    }
    public sealed class SecondExamDateLookupDto
    {
        public int Id { get; set; }
        public string SecondPersianDate { get; set; } = string.Empty;
    }
}
