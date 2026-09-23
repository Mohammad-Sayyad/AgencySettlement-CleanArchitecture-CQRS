using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules
{
    public sealed class HekmatSettlementRule : ISettlementCalculationRule
    {
        public bool CanHandle(int registrationPlanId)
            => registrationPlanId == 2;

        public GroupCalculationResult Calculate(
            SettlementCalculationContext.GroupData group,
            SettlementCalculationContext.Plan5QuotaState quotaState)
        {
            if (group.UnitPrice is null)
            {
                return GroupCalculationResult.Invalid();
            }

            var baseAmount =
                group.CandidateCount *
                group.UnitPrice.Value;

            return GroupCalculationResult.Credit(
                group.CandidateCount,
                group.UnitPrice.Value,
                baseAmount);
        }
    }
}
