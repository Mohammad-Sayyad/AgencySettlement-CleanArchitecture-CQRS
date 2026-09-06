using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class ImportExamResult
    {
        public int TotalReceived { get; set; }
        public int NewRecords { get; set; }
        public int DuplicateRecords { get; set; }
    }
}
