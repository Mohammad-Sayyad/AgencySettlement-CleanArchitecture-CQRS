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
    public sealed class HekmatSettlementRuleTests
    {
        private readonly HekmatSettlementRule _rule = new();

        [Fact]
        public void CanHandle_ShouldReturnTrue_ForPlan2()
        {
            Assert.True(_rule.CanHandle(2));
        }

        [Fact]
        public void CanHandle_ShouldReturnFalse_ForOtherPlans()
        {
            Assert.False(_rule.CanHandle(1));
            Assert.False(_rule.CanHandle(3));
            Assert.False(_rule.CanHandle(5));
            Assert.False(_rule.CanHandle(8));
        }

        [Fact]
        public void Calculate_ShouldCreateCredit()
        {
            var group =
                CreateGroup(
                    candidateCount: 4,
                    unitPrice: 1_400_000m);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.True(result.IsValid);

            Assert.Equal(4, result.PaidCandidateCount);
            Assert.Equal(0, result.FreeCandidateCount);

            Assert.Equal(1_400_000m, result.UnitPrice);
            Assert.Equal(5_600_000m, result.BaseAmount);

            Assert.Equal(0m, result.DebitAmount);
            Assert.Equal(5_600_000m, result.CreditAmount);
            Assert.Equal(5_600_000m, result.AgencyAmount);
            Assert.Equal(5_600_000m, result.GajAmount);
        }

        [Fact]
        public void Calculate_WhenPriceIsMissing_ShouldBeInvalid()
        {
            var group =
                CreateGroup(
                    candidateCount: 4,
                    unitPrice: null);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.False(result.IsValid);
        }

        private static SettlementCalculationContext.GroupData CreateGroup(
            int candidateCount,
            decimal? unitPrice)
        {
            return new SettlementCalculationContext.GroupData(
                1,
                12,
                0,
                2,
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
