using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Data;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Persistence;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation
{
    public sealed class SettlementCalculationCoordinator
    {
        private readonly SettlementCalculationDataLoader _dataLoader;
        private readonly SettlementCalculationPersistence _persistence;
        private readonly IReadOnlyList<ISettlementCalculationRule> _rules;

        public SettlementCalculationCoordinator(
            SettlementCalculationDataLoader dataLoader,
            SettlementCalculationPersistence persistence,
            IEnumerable<ISettlementCalculationRule> rules)
        {
            _dataLoader = dataLoader;
            _persistence = persistence;
            _rules = rules.ToList();
        }

        public async Task<SettlementResultDto> CalculateAsync(
            CalculateSettlementRequest request,
            CancellationToken cancellationToken)
        {
            var context =
                await _dataLoader.LoadAsync(
                    request,
                    cancellationToken);

            var calculatedOrder =
                await CalculateOrderAsync(
                    context,
                    cancellationToken);

            return await _persistence.PersistAsync(
                context,
                calculatedOrder,
                cancellationToken);
        }

        private async Task<Domain.Entities.SettlementOrder>
            CalculateOrderAsync(
                SettlementCalculationContext context,
                CancellationToken cancellationToken)
        {
            var existingOrder =
                context.ExistingOrder;

            var existingPaidAmount =
                0m;

            if (existingOrder is not null)
            {
                existingPaidAmount =
                    existingOrder.TotalCredit -
                    existingOrder.Items.Sum(
                        x => x.CreditAmount);

                if (existingPaidAmount < 0m)
                {
                    existingPaidAmount = 0m;
                }

                existingOrder.Items.Clear();
            }

            var order =
                existingOrder
                ?? new Domain.Entities.SettlementOrder
                {
                    AgencyId =
                        context.Agency.Id,

                    YearId =
                        context.OrderRecords[0].YearId,

                    RegistrationOrder =
                        context.Input.RegistrationOrder,

                    CreatedAt =
                        DateTime.UtcNow
                };

            decimal totalDebit = 0m;
            decimal totalCredit = 0m;
            decimal totalCreditGaj = 0m;

            foreach (var group in context.Groups)
            {
                var rule =
                    _rules.FirstOrDefault(
                        x => x.CanHandle(
                            group.RegistrationPlanId));

                if (rule is null)
                {
                    continue;
                }

                var calculation =
                    rule.Calculate(
                        group,
                        context.QuotaState);

                if (!calculation.IsValid)
                {
                    continue;
                }

                totalDebit +=
                    calculation.DebitAmount;

                totalCredit +=
                    calculation.CreditAmount;

                totalCreditGaj +=
                    calculation.TotalCreditGaj;

                order.Items.Add(
                    CreateSettlementOrderItem(
                        group,
                        calculation,
                        context.Agency));
            }

            if (order.Items.Count == 0)
            {
                throw new InvalidOperationException(
                    $"برای Order {context.Input.RegistrationOrder} هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
            }

            order.PersianExecutionDate =
                context.OrderRecords[0].PersianExecutionDate;

            order.ContractFloorAmount =
                context.Agency.ContractFloorAmount;

            order.TotalDebit =
                Math.Max(
                    0m,
                    totalDebit);

            order.TotalCredit =
                Math.Max(
                    0m,
                    totalCredit +
                    existingPaidAmount);

            order.Balance =
                order.TotalDebit -
                order.TotalCredit;

            order.TotalDebitGaj =
                totalCredit;

            order.TotalCreditGaj =
                totalCreditGaj;

            order.BalanceGaj =
                Math.Max(
                    0m,
                    order.TotalCreditGaj -
                    order.TotalDebit);

            if (existingOrder is null)
            {
                context.Order1 =
                    context.Input.RegistrationOrder == 1
                        ? order
                        : context.Order1;

                context.Order2 =
                    context.Input.RegistrationOrder == 2
                        ? order
                        : context.Order2;
            }

            return order;
        }

        private static Domain.Entities.SettlementOrderItem
            CreateSettlementOrderItem(
                SettlementCalculationContext.GroupData group,
                GroupCalculationResult calculation,
                Domain.Entities.Agency agency)
        {
            return new Domain.Entities.SettlementOrderItem
            {
                SettlementOrderId = 0,

                PackageId =
                    group.PackageId,

                EducationalLevelId =
                    group.EducationalLevelId,

                ExamModeId =
                    group.ExamModeId,

                RegistrationPlanId =
                    group.RegistrationPlanId,

                YearId =
                    group.YearId,

                StudyFieldId =
                    group.StudyFieldId,

                PersianExecutionDate =
                    group.PersianExecutionDate,

                AgencyId =
                    group.AgencyId,

                CandidateCount =
                    group.CandidateCount,

                FreeCandidateCount =
                    calculation.FreeCandidateCount,

                PaidCandidateCount =
                    calculation.PaidCandidateCount,

                FreeQuotaCount =
                    group.RegistrationPlanId == 5
                        ? agency.FreeQuotaCount
                        : 0,

                OneHundredThousandQuotaCount =
                    group.RegistrationPlanId == 5
                        ? agency.OneHundredThousandQuotaCount
                        : 0,

                ContractFloorAmount =
                    agency.ContractFloorAmount,

                UnitPrice =
                    calculation.UnitPrice,

                BaseAmount =
                    calculation.BaseAmount,

                AgencyPercent =
                    calculation.AgencyPercent,

                GajPercent =
                    calculation.GajPercent,

                StudentPercent =
                    calculation.StudentPercent,

                AgencyAmount =
                    calculation.AgencyAmount,

                GajAmount =
                    calculation.GajAmount,

                StudentAmount =
                    calculation.StudentAmount,

                DebitAmount =
                    calculation.DebitAmount,

                CreditAmount =
                    calculation.CreditAmount,

                CreatedAt =
                    DateTime.UtcNow
            };
        }
    }
}
