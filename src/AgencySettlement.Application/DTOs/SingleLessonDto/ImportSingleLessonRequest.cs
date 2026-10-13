using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.SingleLessonDto
{
    public sealed class ImportSingleLessonRequest
    {
        public int AgencyId { get; init; }

        public int YearId { get; init; }

        public string PersianExecutionDate { get; init; } = string.Empty;

        public List<ImportSingleLessonItemRequest> Items { get; init; } = [];
    }
}
