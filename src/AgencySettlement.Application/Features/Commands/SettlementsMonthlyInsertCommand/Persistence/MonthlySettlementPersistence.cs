using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Models;
using AgencySettlement.Domain.Entities;

namespace AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Persistence
{
    public sealed class MonthlySettlementPersistence
    {
        private readonly ISettlementOrderRepository _orderRepository;
        private readonly ISettlementRepository _settlementRepository;
        private readonly ISettlementHistoryRepository _historyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MonthlySettlementPersistence(
            ISettlementOrderRepository orderRepository,
            ISettlementRepository settlementRepository,
            ISettlementHistoryRepository historyRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _settlementRepository = settlementRepository;
            _historyRepository = historyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task PersistAsync(
            MonthlySettlementCalculationContext context,
            List<SettlementOrder> orders,
            CancellationToken cancellationToken)
        {
            foreach (var order in orders)
            {
                var key =
                    new MonthlySettlementCalculationContext.OrderKey(
                        order.AgencyId,
                        order.YearId,
                        order.RegistrationOrder,
                        order.PersianExecutionDate);

                if (!context.ExistingOrders.ContainsKey(key))
                {
                    await _orderRepository.AddAsync(
                        order,
                        cancellationToken);
                }
            }

            var settlements =
                BuildSettlements(
                    context,
                    orders);

            foreach (var settlement in settlements)
            {
                var key =
                    new MonthlySettlementCalculationContext.SettlementKey(
                        settlement.AgencyId,
                        settlement.YearId,
                        settlement.PersianExecutionDate);

                if (!context.ExistingSettlements.ContainsKey(key))
                {
                    await _settlementRepository.AddAsync(
                        settlement,
                        cancellationToken);
                }
                else
                {
                    var existingSettlement =
                        context.ExistingSettlements[key];

                    existingSettlement.TotalDebit =
                        settlement.TotalDebit;

                    existingSettlement.TotalCredit =
                        settlement.TotalCredit;

                    existingSettlement.Balance =
                        settlement.Balance;

                    existingSettlement.TotalDebitGaj =
                        settlement.TotalDebitGaj;

                    existingSettlement.TotalCreditGaj =
                        settlement.TotalCreditGaj;

                    existingSettlement.BalanceGaj =
                        settlement.BalanceGaj;

                    existingSettlement.ContractFloorAmount =
                        settlement.ContractFloorAmount;

                    existingSettlement.PersianExecutionDate =
                        settlement.PersianExecutionDate;
                }
            }

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var histories =
                settlements
                    .Select(
                        settlement =>
                            new SettlementHistory
                            {
                                SettlementId =
                                    settlement.Id,

                                AgencyId =
                                    settlement.AgencyId,

                                YearId =
                                    settlement.YearId,

                                TotalDebit =
                                    settlement.TotalDebit,

                                TotalCredit =
                                    settlement.TotalCredit,

                                Balance =
                                    settlement.Balance,

                                CreatedAt =
                                    DateTime.UtcNow,

                                PersianExecutionDate =
                                    settlement.PersianExecutionDate,

                                TotalDebitGaj =
                                    settlement.TotalDebitGaj,

                                TotalCreditGaj =
                                    settlement.TotalCreditGaj,

                                BalanceGaj =
                                    settlement.BalanceGaj,

                                ContractFloorAmount =
                                    settlement.ContractFloorAmount,

                                Description =
                                    $"محاسبه ماهانه Settlement - Month {context.Month}"
                            })
                    .ToList();

            await _historyRepository.AddRangeAsync(
                histories,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        private static List<Settlement> BuildSettlements(
            MonthlySettlementCalculationContext context,
            List<SettlementOrder> calculatedOrders)
        {
            var result =
                new List<Settlement>();

            foreach (var dateGroup in calculatedOrders.GroupBy(
                         x => new
                         {
                             x.AgencyId,
                             x.YearId,
                             x.PersianExecutionDate
                         }))
            {
                var settlementKey =
                    new MonthlySettlementCalculationContext.SettlementKey(
                        dateGroup.Key.AgencyId,
                        dateGroup.Key.YearId,
                        dateGroup.Key.PersianExecutionDate);

                context.ExistingSettlements.TryGetValue(
                    settlementKey,
                    out var settlement);

                settlement ??=
                    new Settlement
                    {
                        AgencyId =
                            dateGroup.Key.AgencyId,

                        YearId =
                            dateGroup.Key.YearId,

                        PersianExecutionDate =
                            dateGroup.Key.PersianExecutionDate,

                        CreatedAt =
                            DateTime.UtcNow
                    };

                var order1 =
                    dateGroup.FirstOrDefault(
                        x => x.RegistrationOrder == 1);

                var order2 =
                    dateGroup.FirstOrDefault(
                        x => x.RegistrationOrder == 2);

                if (order1 is null)
                {
                    context.ExistingOrders.TryGetValue(
                        new MonthlySettlementCalculationContext.OrderKey(
                            dateGroup.Key.AgencyId,
                            dateGroup.Key.YearId,
                            1,
                            dateGroup.Key.PersianExecutionDate),
                        out order1);
                }

                if (order2 is null)
                {
                    context.ExistingOrders.TryGetValue(
                        new MonthlySettlementCalculationContext.OrderKey(
                            dateGroup.Key.AgencyId,
                            dateGroup.Key.YearId,
                            2,
                            dateGroup.Key.PersianExecutionDate),
                        out order2);
                }

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

                if (!result.Contains(settlement))
                {
                    result.Add(settlement);
                }
            }

            return result;
        }
    }
}