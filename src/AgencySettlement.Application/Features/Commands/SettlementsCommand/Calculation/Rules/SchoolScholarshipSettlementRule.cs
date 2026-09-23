using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules
{
    public sealed class SchoolScholarshipSettlementRule : ISettlementCalculationRule
    {
        private const int OnlineExamModeId = 1;

        private const decimal OneHundredThousandTomanInRial =
            1_000_000m;

        public bool CanHandle(int registrationPlanId)
            => registrationPlanId == 3;

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

            var baseAmount =
                group.CandidateCount *
                OneHundredThousandTomanInRial;

            return GroupCalculationResult.Debit(
                freeCandidateCount: 0,
                paidCandidateCount: group.CandidateCount,
                unitPrice: group.UnitPrice.Value,
                baseAmount: baseAmount);
        }
    }
}
