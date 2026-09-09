using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class SettlementDetailsDto
    {
        public long SettlementId { get; set; }
        public List<SettlementDetailItemDto> Items { get; set; } = [];
    }
}
