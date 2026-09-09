using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Enums
{
    public enum SettlementPaymentStatus
    {
        NotReached = 1,
        InPaymentPeriod = 2,
        Expired = 3
    }
}
