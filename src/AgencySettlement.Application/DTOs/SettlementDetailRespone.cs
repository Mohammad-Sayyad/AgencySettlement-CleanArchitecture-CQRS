using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public class SettlementDetailResponse
    {
        public List<SettlementDetailItemDto> Items { get; set; } = [];

        public decimal TotalBaseAmount { get; set; }

        public decimal TotalInPersonFreeAgencyDebit { get; set; }
        public decimal TotalInPersonFreeGajCredit { get; set; }
        public decimal TotalInPersonHekmatAgencyCredit { get; set; }
        public decimal TotalInPersonHekmatGajDebit { get; set; }
        public decimal TotalInPersonSiteAgencyCredit { get; set; }
        public decimal TotalInPersonSiteGajDebit { get; set; }
        public decimal TotalInPersonScholarshipAgencyDebit { get; set; }
        public decimal TotalInPersonScholarshipGajCredit { get; set; }

        public decimal TotalOnlineFreeAgencyDebit { get; set; }
        public decimal TotalOnlineFreeGajCredit { get; set; }
        public decimal TotalOnlineHekmatAgencyCredit { get; set; }
        public decimal TotalOnlineHekmatGajDebit { get; set; }
        public decimal TotalOnlineSiteAgencyCredit { get; set; }
        public decimal TotalOnlineSiteGajDebit { get; set; }
        public decimal TotalOnlineScholarshipAgencyDebit { get; set; }
        public decimal TotalOnlineScholarshipGajCredit { get; set; }

        public decimal TotalOnlineStudentAmount { get; set; }
    }
}
