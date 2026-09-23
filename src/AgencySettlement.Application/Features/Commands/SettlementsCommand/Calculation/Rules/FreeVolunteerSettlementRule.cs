using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules
{
    public sealed class FreeVolunteerSettlementRule : ISettlementCalculationRule
    {
        private const int OnlineExamModeId = 1;

        private const decimal OneHundredThousandTomanInRial =
            1_000_000m;

        public bool CanHandle(int registrationPlanId)
            => registrationPlanId == 5;

        public GroupCalculationResult Calculate(
            SettlementCalculationContext.GroupData group,
            SettlementCalculationContext.Plan5QuotaState quotaState)
        {
            if (group.UnitPrice is null)
            {
                return GroupCalculationResult.Invalid();
            }

            if (group.ExamModeId == OnlineExamModeId)
            {
                return GroupCalculationResult.Free(
                    group.CandidateCount,
                    group.UnitPrice.Value);
            }

            var freeCandidateCount =
                Math.Min(
                    quotaState.RemainingFreeQuota,
                    group.CandidateCount);

            quotaState.RemainingFreeQuota -=
                freeCandidateCount;

            var remainingCandidateCount =
                group.CandidateCount -
                freeCandidateCount;

            var oneHundredThousandCandidateCount =
                Math.Min(
                    quotaState.RemainingOneHundredThousandQuota,
                    remainingCandidateCount);

            quotaState.RemainingOneHundredThousandQuota -=
                oneHundredThousandCandidateCount;

            var normalCandidateCount =
                remainingCandidateCount -
                oneHundredThousandCandidateCount;

            var baseAmount =
                oneHundredThousandCandidateCount *
                OneHundredThousandTomanInRial;

            baseAmount +=
                normalCandidateCount *
                group.UnitPrice.Value;

            var paidCandidateCount =
                group.CandidateCount -
                freeCandidateCount;

            return GroupCalculationResult.Debit(
                freeCandidateCount,
                paidCandidateCount,
                group.UnitPrice.Value,
                baseAmount);
        }
    }
}
