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
    public sealed class SiteSettlementRuleTests
    {
        private readonly SiteSettlementRule _rule = new();

        [Fact]
        public void CanHandle_ShouldReturnTrue_ForPlan8()
        {
            Assert.True(_rule.CanHandle(8));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        public void CanHandle_ShouldReturnFalse_ForOtherPlans(
            int registrationPlanId)
        {
            Assert.False(_rule.CanHandle(registrationPlanId));
        }

        [Fact]
        public void Calculate_ShouldCreateCreditUsingPercentages()
        {
            var group =
                CreateGroup(
                    candidateCount: 10,
                    unitPrice: 3_000_000m,
                    agencyPercent: 40m,
                    gajPercent: 40m,
                    studentPercent: 20m);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.True(result.IsValid);

            Assert.Equal(30_000_000m, result.BaseAmount);

            Assert.Equal(40m, result.AgencyPercent);
            Assert.Equal(40m, result.GajPercent);
            Assert.Equal(20m, result.StudentPercent);

            Assert.Equal(12_000_000m, result.AgencyAmount);
            Assert.Equal(12_000_000m, result.GajAmount);
            Assert.Equal(6_000_000m, result.StudentAmount);

            Assert.Equal(0m, result.DebitAmount);
            Assert.Equal(12_000_000m, result.CreditAmount);

            Assert.Equal(0m, result.TotalCreditGaj);
        }

        [Fact]
        public void Calculate_WhenPriceIsMissing_ShouldBeInvalid()
        {
            var group =
                CreateGroup(
                    candidateCount: 10,
                    unitPrice: null);

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
                new SettlementCalculationContext.GroupData(
                    1,
                    12,
                    1,
                    8,
                    1,
                    10,
                    106,
                    "05/05/23",
                    10,
                    3_000_000m,
                    null);

            var result =
                _rule.Calculate(
                    group,
                    new SettlementCalculationContext.Plan5QuotaState());

            Assert.False(result.IsValid);
        }

        private static SettlementCalculationContext.GroupData CreateGroup(
            int candidateCount,
            decimal? unitPrice,
            decimal agencyPercent = 40m,
            decimal gajPercent = 40m,
            decimal studentPercent = 20m)
        {
            var percent =
                new Percent
                {
                    AgencyPercent = agencyPercent,
                    GajPercent = gajPercent,
                    StudentPercent = studentPercent
                };

            return new SettlementCalculationContext.GroupData(
                1,
                12,
                1,
                8,
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
