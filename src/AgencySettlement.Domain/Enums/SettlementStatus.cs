using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Domain.Enums
{
   
        public enum SettlementStatus : byte
        {
        Debtor = 1,
        Settled = 2,
        Creditor = 3
    }
    
}
