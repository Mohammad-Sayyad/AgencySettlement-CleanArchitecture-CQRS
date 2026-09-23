using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Persistence
{
    public sealed class SettlementCalculationPersistence
    {
        private readonly ISettlementRepository _settlementRepository;
        private readonly ISettlementOrderRepository _settlementOrderRepository;
        private readonly ISettlementHistoryRepository _historyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SettlementCalculationPersistence(
            ISettlementRepository settlementRepository,
            ISettlementOrderRepository settlementOrderRepository,
            ISettlementHistoryRepository historyRepository,
            IUnitOfWork unitOfWork)
        {
            _settlementRepository = settlementRepository;
            _settlementOrderRepository = settlementOrderRepository;
            _historyRepository = historyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SettlementResultDto> PersistAsync(
            SettlementCalculationContext context,
            SettlementOrder calculatedOrder,
            CancellationToken cancellationToken)
        {
            if (context.ExistingOrder is null)
            {
                await _settlementOrderRepository.AddAsync(
                    calculatedOrder,
                    cancellationToken);
            }

            UpdateSettlement(
                context,
                calculatedOrder);

            var settlement =
                context.Settlement;

            if (settlement is null)
            {
                settlement =
                    new Settlement
                    {
                        AgencyId =
                            context.Input.AgencyId,

                        YearId =
                            context.Input.YearId,

                        CreatedAt =
                            DateTime.UtcNow
                    };

                context.Settlement =
                    settlement;

                await _settlementRepository.AddAsync(
                    settlement,
                    cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var history =
                new SettlementHistory
                {
                    SettlementId =
                        settlement.Id,

                    AgencyId =
                        settlement.AgencyId,

                    YearId =
                        settlement.YearId,

                    PersianExecutionDate =
                        settlement.PersianExecutionDate,

                    ContractFloorAmount =
                        settlement.ContractFloorAmount,

                    TotalDebit =
                        settlement.TotalDebit,

                    TotalCredit =
                        settlement.TotalCredit,

                    Balance =
                        settlement.Balance,

                    TotalDebitGaj =
                        settlement.TotalDebitGaj,

                    TotalCreditGaj =
                        settlement.TotalCreditGaj,

                    BalanceGaj =
                        settlement.BalanceGaj,

                    CreatedAt =
                        DateTime.UtcNow,

                    Description =
                        $"محاسبه Settlement نماینده - Order {context.Input.RegistrationOrder}"
                };

            await _historyRepository.AddAsync(
                history,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return CreateResult(
                settlement,
                calculatedOrder);
        }

        private static void UpdateSettlement(
            SettlementCalculationContext context,
            SettlementOrder calculatedOrder)
        {
            var order1 =
                context.Order1;

            var order2 =
                context.Order2;

            context.Settlement ??=
                new Settlement
                {
                    AgencyId =
                        context.Input.AgencyId,

                    YearId =
                        context.Input.YearId,

                    CreatedAt =
                        DateTime.UtcNow
                };

            var settlement =
                context.Settlement;

            settlement.TotalDebit =
                (order1?.TotalDebit ?? 0m) +
                (order2?.TotalDebit ?? 0m);

            settlement.TotalCredit =
                (order1?.TotalCredit ?? 0m) +
                (order2?.TotalCredit ?? 0m);

            settlement.Balance =
                settlement.TotalDebit -
                settlement.TotalCredit;

            settlement.TotalDebitGaj =
                (order1?.TotalDebitGaj ?? 0m) +
                (order2?.TotalDebitGaj ?? 0m);

            settlement.TotalCreditGaj =
                (order1?.TotalCreditGaj ?? 0m) +
                (order2?.TotalCreditGaj ?? 0m);

            settlement.BalanceGaj =
                Math.Max(
                    0m,
                    settlement.TotalCreditGaj -
                    settlement.TotalDebit);

            var latestOrder =
                order2 ??
                order1;

            if (latestOrder is not null)
            {
                settlement.ContractFloorAmount =
                    latestOrder.ContractFloorAmount;

                settlement.PersianExecutionDate =
                    latestOrder.PersianExecutionDate;
            }
        }

        private static SettlementResultDto CreateResult(
            Settlement settlement,
            SettlementOrder calculatedOrder)
        {
            var items =
                calculatedOrder.Items
                    .Select(
                        x => new SettlementItemResultDto
                        {
                            PackageId =
                                x.PackageId,

                            EducationalLevelId =
                                x.EducationalLevelId,

                            StudyFieldId =
                                x.StudyFieldId,

                            ExamModeId =
                                x.ExamModeId,

                            RegistrationPlanId =
                                x.RegistrationPlanId,

                            YearId =
                                x.YearId,

                            CandidateCount =
                                x.CandidateCount,

                            FreeCandidateCount =
                                x.FreeCandidateCount,

                            PaidCandidateCount =
                                x.PaidCandidateCount,

                            FreeQuotaCount =
                                x.FreeQuotaCount,

                            OneHundredThousandQuotaCount =
                                x.OneHundredThousandQuotaCount,

                            ContractFloorAmount =
                                x.ContractFloorAmount,

                            UnitPrice =
                                x.UnitPrice,

                            BaseAmount =
                                x.BaseAmount,

                            AgencyPercent =
                                x.AgencyPercent,

                            GajPercent =
                                x.GajPercent,

                            StudentPercent =
                                x.StudentPercent,

                            AgencyAmount =
                                x.AgencyAmount,

                            GajAmount =
                                x.GajAmount,

                            StudentAmount =
                                x.StudentAmount,

                            DebitAmount =
                                x.DebitAmount,

                            CreditAmount =
                                x.CreditAmount,

                            PersianExecutionDate =
                                x.PersianExecutionDate
                        })
                    .ToList();

            return new SettlementResultDto
            {
                SettlementId =
                    settlement.Id,

                AgencyId =
                    settlement.AgencyId,

                ContractFloorAmount =
                    settlement.ContractFloorAmount,

                TotalDebit =
                    settlement.TotalDebit,

                TotalCredit =
                    settlement.TotalCredit,

                Balance =
                    settlement.Balance,

                TotalDebitGaj =
                    settlement.TotalDebitGaj,

                TotalCreditGaj =
                    settlement.TotalCreditGaj,

                BalanceGaj =
                    settlement.BalanceGaj,

                PersianExecutionDate =
                    settlement.PersianExecutionDate,

                Items =
                    items
            };
        }
    }
}
