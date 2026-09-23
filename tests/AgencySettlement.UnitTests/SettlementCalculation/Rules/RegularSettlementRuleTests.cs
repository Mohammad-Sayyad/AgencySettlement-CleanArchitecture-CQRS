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
    public sealed class RegularSettlementRuleTests
    {
        private readonly RegularSettlementRule _rule = new();

        [Fact]
        public void CanHandle_ShouldReturnTrue_ForPlan1()
        {
            Assert.True(_rule.CanHandle(1));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(8)]
        public void CanHandle_ShouldReturnFalse_ForOtherPlans(
            int registrationPlanId)
        {
            Assert.False(_rule.CanHandle(registrationPlanId));
        }

        [Fact]
        public void Calculate_ShouldCalculateAgencyAndGajShares()
        {

            var group = CreateGroup(

                candidateCount: 10,
                unitPrice: 5_000_000m,
                percent: new Percent
                {
                    AgencyPercent = 55m,
                    GajPercent = 45m,
                    StudentPercent = 0m
                }
                );
            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.True(result.IsValid);

            Assert.Equal(50_000_000m, result.BaseAmount);

            Assert.Equal(55m, result.AgencyPercent);
            Assert.Equal(45m, result.GajPercent);
            Assert.Equal(0m, result.StudentPercent);

            Assert.Equal(27_500_000m, result.AgencyAmount);
            Assert.Equal(22_500_000m, result.GajAmount);
        }

        [Fact]
        public void Calculate_WhenPriceIsMissing_ShouldBeInvalid()
        {
            var group =
                CreateGroup(
                    candidateCount: 10,
                    unitPrice: null,
                    agencyPercent: 55m,
                    gajPercent: 45m,
                    studentPercent: 0m);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Calculate_WhenPercentIsMissing_ShouldBeInvalid()
        {
            var group =
                CreateGroup(
                    candidateCount: 10,
                    unitPrice: 5_000_000m,
                    percent: null);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.False(result.IsValid);
        }

        private static SettlementCalculationContext.GroupData CreateGroup(
    int candidateCount,
    decimal? unitPrice,
    decimal agencyPercent = 55m,
    decimal gajPercent = 45m,
    decimal studentPercent = 0m,
    Percent? percent = null)
        {
            return new SettlementCalculationContext.GroupData(
                1,
                12,
                0,
                1,
                1,
                10,
                106,
                "05/05/23",
                candidateCount,
                unitPrice,
                percent);
        }
    }
}
