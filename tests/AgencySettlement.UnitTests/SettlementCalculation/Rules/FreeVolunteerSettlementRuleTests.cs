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
    public sealed class FreeVolunteerSettlementRuleTests
    {
        private readonly FreeVolunteerSettlementRule _rule = new();

        [Fact]
        public void CanHandle_ShouldReturnTrue_ForPlan5()
        {
            Assert.True(_rule.CanHandle(5));
        }

        [Fact]
        public void CanHandle_ShouldReturnFalse_ForOtherPlans()
        {
            Assert.False(_rule.CanHandle(1));
            Assert.False(_rule.CanHandle(2));
            Assert.False(_rule.CanHandle(3));
            Assert.False(_rule.CanHandle(8));
        }

        [Fact]
        public void Calculate_Online_ShouldBeFree_WithoutConsumingQuota()
        {
            var quota =
                CreateQuota(
                    freeQuota: 2,
                    oneHundredThousandQuota: 3);

            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 1,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(group, quota);

            Assert.True(result.IsValid);

            Assert.Equal(5, result.FreeCandidateCount);
            Assert.Equal(0, result.PaidCandidateCount);

            Assert.Equal(0m, result.BaseAmount);
            Assert.Equal(0m, result.DebitAmount);

            Assert.Equal(2, quota.RemainingFreeQuota);
            Assert.Equal(3, quota.RemainingOneHundredThousandQuota);
        }

        [Fact]
        public void Calculate_ShouldConsumeFreeQuotaFirst()
        {
            var quota =
                CreateQuota(
                    freeQuota: 2,
                    oneHundredThousandQuota: 3);

            var group =
                CreateGroup(
                    candidateCount: 2,
                    examModeId: 0,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(group, quota);

            Assert.True(result.IsValid);

            Assert.Equal(2, result.FreeCandidateCount);
            Assert.Equal(0, result.PaidCandidateCount);

            Assert.Equal(0m, result.BaseAmount);

            Assert.Equal(0, quota.RemainingFreeQuota);
            Assert.Equal(3, quota.RemainingOneHundredThousandQuota);
        }

        [Fact]
        public void Calculate_ShouldConsumeFreeQuota_ThenOneHundredThousandQuota()
        {
            var quota =
                CreateQuota(
                    freeQuota: 2,
                    oneHundredThousandQuota: 3);

            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 0,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(group, quota);

            Assert.True(result.IsValid);

            Assert.Equal(2, result.FreeCandidateCount);
            Assert.Equal(3, result.PaidCandidateCount);

            Assert.Equal(3_000_000m, result.BaseAmount);
            Assert.Equal(3_000_000m, result.DebitAmount);

            Assert.Equal(0, quota.RemainingFreeQuota);
            Assert.Equal(0, quota.RemainingOneHundredThousandQuota);
        }

        [Fact]
        public void Calculate_WhenQuotasAreExhausted_ShouldUseNormalPrice()
        {
            var quota =
                CreateQuota(
                    freeQuota: 0,
                    oneHundredThousandQuota: 0);

            var group =
                CreateGroup(
                    candidateCount: 3,
                    examModeId: 0,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(group, quota);

            Assert.True(result.IsValid);

            Assert.Equal(0, result.FreeCandidateCount);
            Assert.Equal(3, result.PaidCandidateCount);

            Assert.Equal(15_000_000m, result.BaseAmount);
            Assert.Equal(15_000_000m, result.DebitAmount);
        }

        [Fact]
        public void Calculate_WhenFreeQuotaIsPartiallyAvailable_ShouldUseRemainingCandidatesAsPaid()
        {
            var quota =
                CreateQuota(
                    freeQuota: 2,
                    oneHundredThousandQuota: 0);

            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 0,
                    unitPrice: 5_000_000m);

            var result =
                _rule.Calculate(group, quota);

            Assert.True(result.IsValid);

            Assert.Equal(2, result.FreeCandidateCount);
            Assert.Equal(3, result.PaidCandidateCount);

            Assert.Equal(15_000_000m, result.BaseAmount);
            Assert.Equal(15_000_000m, result.DebitAmount);

            Assert.Equal(0, quota.RemainingFreeQuota);
        }

        [Fact]
        public void Calculate_WhenPriceIsMissing_ShouldBeInvalid()
        {
            var quota =
                CreateQuota(2, 3);

            var group =
                CreateGroup(
                    candidateCount: 5,
                    examModeId: 0,
                    unitPrice: null);

            var result =
                _rule.Calculate(group, quota);

            Assert.False(result.IsValid);

            Assert.Equal(2, quota.RemainingFreeQuota);
            Assert.Equal(3, quota.RemainingOneHundredThousandQuota);
        }

        private static SettlementCalculationContext.Plan5QuotaState CreateQuota(
            int freeQuota,
            int oneHundredThousandQuota)
        {
            var quota =
                new SettlementCalculationContext.Plan5QuotaState();

            quota.Initialize(
                freeQuota,
                oneHundredThousandQuota);

            return quota;
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
                5,
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
