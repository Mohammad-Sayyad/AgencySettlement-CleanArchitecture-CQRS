using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AgencySettlement.UnitTests.SettlementCalculation.Rules
{
    public sealed class SchoolScholarshipSettlementRuleTests
    {
        private readonly SchoolScholarshipSettlementRule _rule = new();

        [Fact]
        public void CanHandle_ShouldReturnTrue_ForPlan3()
        {
            Assert.True(_rule.CanHandle(3));
        }

        [Fact]
        public void Calculate_Online_ShouldBeFree()
        {
            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 1,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.True(result.IsValid);

            Assert.Equal(5, result.FreeCandidateCount);
            Assert.Equal(0, result.PaidCandidateCount);

            Assert.Equal(0m, result.BaseAmount);
            Assert.Equal(0m, result.DebitAmount);
            Assert.Equal(0m, result.CreditAmount);
        }

        [Fact]
        public void Calculate_InPerson_ShouldChargeOneHundredThousandPerCandidate()
        {
            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 0,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.True(result.IsValid);

            Assert.Equal(0, result.FreeCandidateCount);
            Assert.Equal(5, result.PaidCandidateCount);

            Assert.Equal(5_000_000m, result.BaseAmount);
            Assert.Equal(5_000_000m, result.DebitAmount);

            Assert.Equal(0m, result.CreditAmount);
        }

        [Fact]
        public void Calculate_WhenPriceIsMissing_ShouldBeInvalid()
        {
            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 0,
                    unitPrice: null);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.False(result.IsValid);
        }

        private static SettlementCalculationContext.GroupData CreateGroup(
            int candidateCount,
            int examModeId,
            decimal? unitPrice)
        {
            return new SettlementCalculationContext.GroupData(
                1,
                12,
                examModeId,
                3,
                1,
                10,
                106,
                "05/05/23",
                candidateCount,
                unitPrice,
                null);
        }
    }
}
