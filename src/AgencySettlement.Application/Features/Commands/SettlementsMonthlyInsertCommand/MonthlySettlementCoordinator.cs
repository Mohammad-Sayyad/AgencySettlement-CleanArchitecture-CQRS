using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Models;
using AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation.Rules;
using AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Models;
using AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Persistence;
using AgencySettlement.Domain.Entities;

namespace AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand
{
    public sealed class MonthlySettlementCoordinator
    {
        private readonly IExternalExamRecordRepository _externalRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IPercentRuleRepository _percentRepository;
        private readonly ISettlementOrderRepository _orderRepository;
        private readonly ISettlementRepository _settlementRepository;
        private readonly IReadOnlyList<ISettlementCalculationRule> _rules;

        public MonthlySettlementCoordinator(
            IExternalExamRecordRepository externalRepository,
            IAgencyRepository agencyRepository,
            IPriceRepository priceRepository,
            IPercentRuleRepository percentRepository,
            ISettlementOrderRepository orderRepository,
            ISettlementRepository settlementRepository,
            IEnumerable<ISettlementCalculationRule> rules)
        {
            _externalRepository = externalRepository;
            _agencyRepository = agencyRepository;
            _priceRepository = priceRepository;
            _percentRepository = percentRepository;
            _orderRepository = orderRepository;
            _settlementRepository = settlementRepository;
            _rules = rules.ToList();
        }

        public async Task<MonthlySettlementBatchResult> CalculateAsync(
            int yearId,
            string month,
            CancellationToken cancellationToken)
        {
            Validate(yearId, month);

            var context =
                await BuildContextAsync(
                    yearId,
                    month,
                    cancellationToken);

            var calculatedOrders =
                CalculateOrders(
                    context,
                    context.Records);

            if (calculatedOrders.Count == 0)
            {
                throw new InvalidOperationException(
                    "هیچ SettlementOrder قابل محاسبه‌ای ایجاد نشد.");
            }

            var settlements =
                BuildSettlements(
                    context,
                    calculatedOrders);

            return CreateBatchResult(
                context,
                calculatedOrders,
                settlements.Count);
        }

        public async Task<List<SettlementOrder>> CalculateOrdersAsync(
            int yearId,
            string month,
            CancellationToken cancellationToken)
        {
            Validate(yearId, month);

            var context =
                await BuildContextAsync(
                    yearId,
                    month,
                    cancellationToken);

            return CalculateOrders(
                context,
                context.Records);
        }

        public async Task<MonthlySettlementBatchResult> CalculateAndPersistAsync(
            int yearId,
            string month,
            MonthlySettlementPersistence persistence,
            CancellationToken cancellationToken)
        {
            Validate(yearId, month);

            var context =
                await BuildContextAsync(
                    yearId,
                    month,
                    cancellationToken);

            var orders =
                CalculateOrders(
                    context,
                    context.Records);

            if (orders.Count == 0)
            {
                throw new InvalidOperationException(
                    "هیچ SettlementOrder قابل محاسبه‌ای ایجاد نشد.");
            }

            await persistence.PersistAsync(
                context,
                orders,
                cancellationToken);

            var settlements =
                BuildSettlements(
                    context,
                    orders);

            return CreateBatchResult(
                context,
                orders,
                settlements.Count);
        }

        private async Task<MonthlySettlementCalculationContext> BuildContextAsync(
            int yearId,
            string month,
            CancellationToken cancellationToken)
        {
            var records =
                await _externalRepository.GetForMonthlyCalculationAsync(
                    yearId,
                    month,
                    cancellationToken);

            if (records.Count == 0)
            {
                throw new InvalidOperationException(
                    $"هیچ رکوردی برای سال {yearId} و تاریخ {month} پیدا نشد.");
            }

            var agencyIds =
                records
                    .Select(x => x.AgencyId)
                    .Distinct()
                    .ToList();

            var agencies =
                await _agencyRepository.GetByIdsAsync(
                    agencyIds,
                    cancellationToken);

            var agencyDictionary =
                agencies.ToDictionary(x => x.Id);

            ValidateAgencies(
                agencyIds,
                agencyDictionary);

            var prices =
                await _priceRepository.GetForMonthlyCalculationAsync(
                    yearId,
                    cancellationToken);

            var percents =
                await _percentRepository.GetForMonthlyCalculationAsync(
                    agencyIds,
                    cancellationToken);

            var existingOrders =
                await _orderRepository.GetForMonthlyCalculationAsync(
                    yearId,
                    agencyIds,
                    cancellationToken);

            var existingSettlements =
                await _settlementRepository.GetForMonthlyCalculationAsync(
                    yearId,
                    agencyIds,
                    cancellationToken);

            var existingOrderDictionary =
     existingOrders.ToDictionary(
         x => new MonthlySettlementCalculationContext.OrderKey(
             x.AgencyId,
             x.YearId,
             x.RegistrationOrder,
             x.PersianExecutionDate));

            ValidateOrders(
                records,
                existingOrderDictionary);

            var existingSettlementDictionary =
                existingSettlements.ToDictionary(
                    x => new MonthlySettlementCalculationContext.SettlementKey(
                        x.AgencyId,
                        x.YearId,
                        x.PersianExecutionDate));

            return new MonthlySettlementCalculationContext
            {
                YearId = yearId,
                Month = month,
                Records = records,
                Agencies = agencyDictionary,
                Prices = BuildPriceDictionary(prices),
                Percents = BuildPercentDictionary(percents),
                ExistingOrders = existingOrderDictionary,
                ExistingSettlements = existingSettlementDictionary
            };
        }

        private List<SettlementOrder> CalculateOrders(
            MonthlySettlementCalculationContext context,
            List<ExternalExamRecord> records)
        {
            var calculatedOrders =
                new List<SettlementOrder>();

            foreach (var dateGroup in records.GroupBy(
                         x => new
                         {
                             x.AgencyId,
                             x.PersianExecutionDate
                         }))
            {
                var agency =
                    context.Agencies[
                        dateGroup.Key.AgencyId];

                var quotaState =
                    new SettlementCalculationContext.Plan5QuotaState();

                quotaState.Initialize(
                    agency.FreeQuotaCount,
                    agency.OneHundredThousandQuotaCount);

                foreach (var orderGroup in dateGroup.GroupBy(
                             x => x.RegistrationOrder))
                {
                    var order =
                        CalculateOrder(
                            context,
                            agency,
                            orderGroup.ToList(),
                            orderGroup.Key,
                            quotaState);

                    calculatedOrders.Add(order);
                }
            }

            return calculatedOrders;
        }

        private SettlementOrder CalculateOrder(
            MonthlySettlementCalculationContext context,
            Agency agency,
            List<ExternalExamRecord> records,
            int registrationOrder,
            SettlementCalculationContext.Plan5QuotaState quotaState)
        {
            if (records.Count == 0)
            {
                throw new InvalidOperationException(
                    "رکوردی برای محاسبه Order وجود ندارد.");
            }

            var key =
     new MonthlySettlementCalculationContext.OrderKey(
         agency.Id,
         context.YearId,
         registrationOrder,
         records[0].PersianExecutionDate);


            context.ExistingOrders.TryGetValue(
                key,
                out var existingOrder);

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
                existingOrder ??
                new SettlementOrder
                {
                    AgencyId =
                        agency.Id,

                    YearId =
                        context.YearId,

                    RegistrationOrder =
                        registrationOrder,

                    CreatedAt =
                        DateTime.UtcNow
                };

            decimal totalDebit = 0m;
            decimal totalCredit = 0m;
            decimal totalCreditGaj = 0m;

            var groupedRecords =
                records.GroupBy(x =>
                    new
                    {
                        x.PackageId,
                        x.EducationalLevelId,
                        x.ExamModeId,
                        x.RegistrationPlanId,
                        x.StudyFieldId,
                        x.YearId,
                        x.AgencyId,
                        x.PersianExecutionDate
                    });

            foreach (var group in groupedRecords)
            {
                var first =
                    group.First();

                var priceKey =
                    new MonthlySettlementCalculationContext.PriceKey(
                        first.PackageId,
                        first.EducationalLevelId,
                        first.ExamModeId,
                        first.RegistrationPlanId,
                        first.YearId);

                context.Prices.TryGetValue(
                    priceKey,
                    out var unitPrice);

                Percent? percent = null;

                if (first.RegistrationPlanId is 1 or 8)
                {
                    var percentKey =
                        new MonthlySettlementCalculationContext.PercentKey(
                            first.AgencyId,
                            first.ExamModeId);

                    context.Percents.TryGetValue(
                        percentKey,
                        out percent);
                }

                var groupData =
                    new SettlementCalculationContext.GroupData(
                        first.PackageId,
                        first.EducationalLevelId,
                        first.ExamModeId,
                        first.RegistrationPlanId,
                        first.StudyFieldId,
                        first.YearId,
                        first.AgencyId,
                        first.PersianExecutionDate,
                        group.Count(),
                        unitPrice,
                        percent);

                var rule =
                    _rules.FirstOrDefault(
                        x => x.CanHandle(
                            groupData.RegistrationPlanId));

                if (rule is null)
                {
                    continue;
                }

                var calculation =
                    rule.Calculate(
                        groupData,
                        quotaState);

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
                    CreateOrderItem(
                        groupData,
                        calculation,
                        agency));
            }

            if (order.Items.Count == 0)
            {
                throw new InvalidOperationException(
                    $"برای نماینده {agency.Id} و Order {registrationOrder} هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
            }

            var executionDates =
                records
                    .Select(x => x.PersianExecutionDate)
                    .Distinct()
                    .ToList();

            if (executionDates.Count != 1)
            {
                throw new InvalidOperationException(
                    $"برای نماینده {agency.Id} و Order {registrationOrder} بیش از یک تاریخ اجرای متفاوت وجود دارد. این ساختار SettlementOrder فقط یک PersianExecutionDate برای هر Order نگهداری می‌کند.");
            }

            order.PersianExecutionDate =
                executionDates[0];

            order.ContractFloorAmount =
                agency.ContractFloorAmount;

            order.TotalDebit =
                Math.Max(
                    0m,
                    totalDebit);

            order.TotalCredit =
                Math.Max(
                    0m,
                    totalCredit + existingPaidAmount);

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

            return order;
        }

        private static SettlementOrderItem CreateOrderItem(
            SettlementCalculationContext.GroupData group,
            GroupCalculationResult calculation,
            Agency agency)
        {
            return new SettlementOrderItem
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
                var key =
                    new MonthlySettlementCalculationContext.SettlementKey(
                        dateGroup.Key.AgencyId,
                        dateGroup.Key.YearId,
                        dateGroup.Key.PersianExecutionDate);

                context.ExistingSettlements.TryGetValue(
                    key,
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

                settlement.ContractFloorAmount =
                    dateGroup
                        .Select(x => x.ContractFloorAmount)
                        .FirstOrDefault();

                settlement.PersianExecutionDate =
                    dateGroup.Key.PersianExecutionDate;

                if (!result.Contains(settlement))
                {
                    result.Add(settlement);
                }
            }

            return result;
        }

        private static MonthlySettlementBatchResult CreateBatchResult(
            MonthlySettlementCalculationContext context,
            List<SettlementOrder> orders,
            int settlementCount)
        {
            return new MonthlySettlementBatchResult
            {
                YearId =
                    context.YearId,

                Month =
                    context.Month,

                AgencyCount =
                    context.Records
                        .Select(x => x.AgencyId)
                        .Distinct()
                        .Count(),

                OrderCount =
                    orders.Count,

                OrderItemCount =
                    orders.Sum(x => x.Items.Count),

                SettlementCount =
                    settlementCount,

                HistoryCount =
                    settlementCount,

                TotalDebit =
                    orders.Sum(x => x.TotalDebit),

                TotalCredit =
                    orders.Sum(x => x.TotalCredit),

                TotalDebitGaj =
                    orders.Sum(x => x.TotalDebitGaj),

                TotalCreditGaj =
                    orders.Sum(x => x.TotalCreditGaj)
            };
        }

        private static Dictionary<
            MonthlySettlementCalculationContext.PriceKey,
            decimal?>
            BuildPriceDictionary(
                List<Price> prices)
        {
            return prices
                .GroupBy(x =>
                    new MonthlySettlementCalculationContext.PriceKey(
                        x.PackageId,
                        x.EducationalLevelId,
                        x.ExamModeId,
                        x.RegistrationPlanId,
                        x.YearId))
                .ToDictionary(
                    x => x.Key,
                    x => x
                        .OrderByDescending(
                            p => p.PersianExecutionDate)
                        .Select(p => (decimal?)p.Amount)
                        .FirstOrDefault());
        }

        private static Dictionary<
            MonthlySettlementCalculationContext.PercentKey,
            Percent?>
            BuildPercentDictionary(
                List<Percent> percents)
        {
            return percents
                .GroupBy(x =>
                    new MonthlySettlementCalculationContext.PercentKey(
                        x.AgencyId,
                        x.ExamModeId))
                .ToDictionary(
                    x => x.Key,
                    x => x
                        .OrderByDescending(
                            p => p.PersianExecutionDate)
                        .FirstOrDefault());
        }

        private static void ValidateAgencies(
            List<int> agencyIds,
            Dictionary<int, Agency> agencies)
        {
            var missing =
                agencyIds
                    .Where(x => !agencies.ContainsKey(x))
                    .ToList();

            if (missing.Count == 0)
            {
                return;
            }

            throw new InvalidOperationException(
                $"نماینده‌های زیر پیدا نشدند: {string.Join(", ", missing)}");
        }

        private static void ValidateOrders(
            List<ExternalExamRecord> records,
            Dictionary<
                MonthlySettlementCalculationContext.OrderKey,
                SettlementOrder> existingOrders)
        {
            var invalidOrders =
                records
                    .Where(x => x.RegistrationOrder is not (1 or 2))
                    .Select(x => x.RegistrationOrder)
                    .Distinct()
                    .ToList();

            if (invalidOrders.Count > 0)
            {
                throw new InvalidOperationException(
                    $"RegistrationOrder نامعتبر است: {string.Join(", ", invalidOrders)}");
            }
        }

        private static void Validate(
            int yearId,
            string month)
        {
            if (yearId <= 0)
            {
                throw new ArgumentException(
                    "YearId نامعتبر است.",
                    nameof(yearId));
            }

            if (string.IsNullOrWhiteSpace(month))
            {
                throw new ArgumentException(
                    "Month الزامی است.",
                    nameof(month));
            }
        }
    }
}