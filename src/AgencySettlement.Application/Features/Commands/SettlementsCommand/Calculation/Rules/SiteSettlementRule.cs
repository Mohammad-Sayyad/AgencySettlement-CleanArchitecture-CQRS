using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules
{
    public sealed class SiteSettlementRule : ISettlementCalculationRule
    {
        public bool CanHandle(int registrationPlanId)
            => registrationPlanId == 8;

        public GroupCalculationResult Calculate(
            SettlementCalculationContext.GroupData group,
            SettlementCalculationContext.Plan5QuotaState quotaState)
        {
            if (group.UnitPrice is null || group.Percent is null)
            {
                return GroupCalculationResult.Invalid();
            }

            var baseAmount =
                group.CandidateCount *
                group.UnitPrice.Value;

            var agencyAmount =
                CalculateShare(
                    baseAmount,
                    group.Percent.AgencyPercent);

            var gajAmount =
                CalculateShare(
                    baseAmount,
                    group.Percent.GajPercent);

            var studentAmount =
                CalculateShare(
                    baseAmount,
                    group.Percent.StudentPercent);

            return GroupCalculationResult.Site(
                group.UnitPrice.Value,
                baseAmount,
                group.Percent.AgencyPercent,
                group.Percent.GajPercent,
                group.Percent.StudentPercent,
                agencyAmount,
                gajAmount,
                studentAmount);
        }

        private static decimal CalculateShare(
            decimal baseAmount,
            decimal percent)
        {
            return baseAmount *
                   percent /
                   100m;
        }
    }
}
