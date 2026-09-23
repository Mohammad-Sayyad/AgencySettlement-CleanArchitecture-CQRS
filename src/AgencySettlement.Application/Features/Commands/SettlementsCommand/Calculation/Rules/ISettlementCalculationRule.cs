using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules
{
    public interface ISettlementCalculationRule
    {
        bool CanHandle(int registrationPlanId);

        GroupCalculationResult Calculate(
            SettlementCalculationContext.GroupData group,
            SettlementCalculationContext.Plan5QuotaState quotaState);
    }
}
