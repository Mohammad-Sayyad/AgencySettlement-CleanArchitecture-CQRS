using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs.ExternalDtos
{
    public sealed class SettlementOrderReportDto
    {
        public int AgencyId { get; set; }
        public int YearId { get; set; }
        public int RegistrationOrder { get; set; }

        public string PreviousExecutionDate { get; set; } = string.Empty;
        public string PersianExecutionDate { get; set; } = string.Empty;

        public int TotalCandidateCount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SettlementOrderReportItemDto> Items { get; set; } = [];
    }
}
