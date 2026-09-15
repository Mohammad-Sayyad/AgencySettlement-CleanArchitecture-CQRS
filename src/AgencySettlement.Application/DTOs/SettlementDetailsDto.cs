using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{

    public sealed class SettlementDetailsDto
    {
        public List<SettlementDetailItemDto> Items { get; set; } = [];
        public decimal TotalBaseAmount { get; set; }
        //
        public int TotalPersonStudentCountFree { get; set; }
        public decimal TotalInPersonFreeAgencyDebit { get; set; }
        public decimal TotalInPersonFreeGajCredit { get; set; }
        public decimal TotalOnlineStudentAmount { get; set; }

        //
        public decimal TotalInPersonHekmatAgencyCredit { get; set; }
        public decimal TotalInPersonHekmatGajDebit { get; set; }
        public int TotalStudentPersonHekmat { get; set; }

        //
        public decimal TotalInPersonSiteAgencyCredit { get; set; }
        public decimal TotalInPersonSiteGajDebit { get; set; }
        public int TotalStudentPersonSite { get; set; }

        //
        public decimal TotalInPersonScholarshipAgencyDebit { get; set; }
        public decimal TotalInPersonScholarshipGajCredit { get; set; }

        public int TotalStudentPersonScholarship { get; set; }

        public int TotalInPersonFreeQuota { get; set; }
        public int TotalInPersonOneHundredThousandQuota { get; set; }

        public int TotalInPersonFreeQuotaUsed { get; set; }
        public int TotalInPersonOneHundredThousandQuotaUsed { get; set; }

        //
        public decimal TotalOnlineFreeAgencyDebit { get; set; }
        public decimal TotalOnlineFreeGajCredit { get; set; }
        public int TotalStudentOnlineFree { get; set; }

        //
        public decimal TotalOnlineHekmatAgencyCredit { get; set; }
        public decimal TotalOnlineHekmatGajDebit { get; set; }
        public int TotalStudentOnlineHekmat { get; set; }

        //
        public decimal TotalOnlineSiteAgencyCredit { get; set; }
        public decimal TotalOnlineSiteGajDebit { get; set; }
        public int TotalStudentOnlineSite { get; set; }

        //
        public decimal TotalOnlineScholarshipAgencyCredit { get; set; }
        public decimal TotalOnlineScholarshipGajCredit { get; set; }
        public int TotalStudentOnlineScholarship { get; set; }


        //
        public decimal ContractFloorAmount { get; set; }
        public decimal TotalDebit { get; set; }
    }
  
}
