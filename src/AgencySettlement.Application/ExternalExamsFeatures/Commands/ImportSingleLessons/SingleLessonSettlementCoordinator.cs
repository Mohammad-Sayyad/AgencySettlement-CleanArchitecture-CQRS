
using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons.Rules;
using AgencySettlement.Domain.Entities.SingleLesson;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons;

public interface ISingleLessonSettlementCoordinator
{
    Task RecalculateAsync(
        IEnumerable<SingleLessonSettlementGroup> groups,
        CancellationToken ct);
}

public sealed class SingleLessonSettlementCoordinator
    : ISingleLessonSettlementCoordinator
{
    private static readonly HashSet<int> EligiblePlans = [1, 2, 8];

    private readonly ISingleLessonSettlementStore _store;
    private readonly ISingleLessonPlanRuleFactory _ruleFactory;
    private readonly ILogger<SingleLessonSettlementCoordinator> _logger;

    public SingleLessonSettlementCoordinator(
        ISingleLessonSettlementStore store,
        ISingleLessonPlanRuleFactory ruleFactory,
        ILogger<SingleLessonSettlementCoordinator> logger)
    {
        _store = store;
        _ruleFactory = ruleFactory;
        _logger = logger;
    }

    public async Task RecalculateAsync(
        IEnumerable<SingleLessonSettlementGroup> groups,
        CancellationToken ct)
    {




        ArgumentNullException.ThrowIfNull(groups);



        var distinctGroups = groups
            .Where(x =>
                x.AgencyId > 0 &&
                x.YearId > 0 &&
                !string.IsNullOrWhiteSpace(x.PersianExecutionDate))
            .Distinct()
            .ToList();

        if (distinctGroups.Count == 0)
            return;

        var groupInputs =
            new List<(SingleLessonSettlementGroup Key,
                List<SingleLessonExternalInput> Inputs)>();

        foreach (var key in distinctGroups)
        {
            ct.ThrowIfCancellationRequested();

            var inputs = (await _store.GetInputsAsync(
                    key.AgencyId,
                    key.YearId,
                    key.PersianExecutionDate,
                    ct))
                .Where(x => EligiblePlans.Contains(x.RegistrationPlanId))
                .ToList();

            groupInputs.Add((key, inputs));
        }

        var allInputs = groupInputs
            .SelectMany(x => x.Inputs)
            .ToList();

        // قیمت‌ها و درصدها را گروهی می‌خوانیم، نه برای هر ورودی جداگانه.
        var prices = await _store.GetPricesAsync(allInputs, ct);
        var percents = await _store.GetPercentsAsync(allInputs, ct);

        var priceByKey = prices
            .GroupBy(p => (
                p.CourseId,
                p.YearId,
                p.PackageId,
                p.StudyFieldId,
                p.EducationalLevelId))
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(p => p.CreatedAt)
                      .ThenByDescending(p => p.Id)
                      .First());

        var percentByKey = percents
            .GroupBy(p => (
                p.CourseId,
                p.YearId,
                p.PackageId,
                p.StudyFieldId,
                p.EducationalLevelId))
            .ToDictionary(
                g => g.Key,
                g => g.ToList());

        foreach (var group in groupInputs)
        {
            ct.ThrowIfCancellationRequested();

            var key = group.Key;
            var inputs = group.Inputs;

            var settlement = await _store.GetSettlementAsync(
                key.AgencyId,
                key.YearId,
                key.PersianExecutionDate,
                ct);

            var isNew = settlement is null;

            if (isNew && inputs.Count == 0)
                continue;

            var now = DateTime.UtcNow;

            settlement ??= new SingleLessonSettlement
            {
                AgencyId = key.AgencyId,
                YearId = key.YearId,
                PersianExecutionDate = key.PersianExecutionDate,
                CreatedAt = now
            };

            // برای تسویه جدید، ابتدا شناسه دیتابیس تولید می‌شود.
            if (isNew)
            {
                await _store.AddSettlementAsync(settlement, ct);
                await _store.SaveChangesAsync(ct);
            }

            var items =
                new List<SingleLessonSettlementItem>(inputs.Count);

            decimal totalDebit = 0m;
            decimal calculatedCredit = 0m;
            decimal totalAgencyAmount = 0m;
            decimal totalGajAmount = 0m;
            var totalCourseCount = 0;

            foreach (var input in inputs)
            {
                ct.ThrowIfCancellationRequested();

                var priceKey = (
                    input.CourseId,
                    input.YearId,
                    input.PackageId,
                    input.StudyFieldId,
                    input.EducationalLevelId);

                if (!priceByKey.TryGetValue(priceKey, out var price))
                {
                    throw new InvalidOperationException(
                        $"قیمت فعال تک‌درس پیدا نشد: " +
                        $"CandidateExamId={input.CandidateExamId}, " +
                        $"ExternalInputId={input.Id}, " +
                        $"CourseId={input.CourseId}, " +
                        $"YearId={input.YearId}, " +
                        $"PackageId={input.PackageId}, " +
                        $"StudyFieldId={input.StudyFieldId}, " +
                        $"EducationalLevelId={input.EducationalLevelId}.");
                }

                if (!percentByKey.TryGetValue(
                        priceKey, out var candidates))
                {
                    throw new InvalidOperationException(
                        $"درصد فعال تک‌درس پیدا نشد: " +
                        $"CourseId={input.CourseId}, " +
                        $"YearId={input.YearId}, " +
                        $"AgencyId={input.AgencyId}.");
                }

                // درصد اختصاصی نمایندگی بر درصد پیش‌فرض اولویت دارد.
                var percent = candidates
                    .Where(p => p.AgencyId == input.AgencyId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ThenByDescending(p => p.Id)
                    .FirstOrDefault()
                    ?? candidates
                        .Where(p => p.AgencyId == null)
                        .OrderByDescending(p => p.CreatedAt)
                        .ThenByDescending(p => p.Id)
                        .FirstOrDefault();

                if (percent is null)
                {
                    throw new InvalidOperationException(
                        $"درصد فعال تک‌درس پیدا نشد: " +
                        $"CourseId={input.CourseId}, " +
                        $"YearId={input.YearId}, " +
                        $"AgencyId={input.AgencyId}.");
                }

                ValidateConfiguration(price, percent, input);

                const int quantity = 1;

                var baseAmount = decimal.Round(
                    price.Amount * quantity,
                    2,
                    MidpointRounding.AwayFromZero);

                var agencyAmount = decimal.Round(
                    baseAmount * percent.AgencyPercent / 100m,
                    2,
                    MidpointRounding.AwayFromZero);

                var gajAmount = baseAmount - agencyAmount;

                var (debit, credit) = _ruleFactory
                    .Get(input.RegistrationPlanId)
                    .Calculate(agencyAmount);

                items.Add(new SingleLessonSettlementItem
                {
                    ExternalInputId = input.Id,
                    AgencyId = input.AgencyId,
                    CourseId = input.CourseId,
                    YearId = input.YearId,
                    PersianExecutionDate = input.PersianExecutionDate,
                    PackageId = input.PackageId,
                    EducationalLevelId = input.EducationalLevelId,
                    ExamModeId = input.ExamModeId,
                    StudyFieldId = input.StudyFieldId,
                    RegistrationPlanId = input.RegistrationPlanId,
                    Quantity = quantity,
                    UnitPrice = price.Amount,
                    BaseAmount = baseAmount,
                    AgencyPercent = percent.AgencyPercent,
                    GajPercent = percent.GajPercent,
                    AgencyAmount = agencyAmount,
                    GajAmount = gajAmount,
                    DebitAmount = debit,
                    CreditAmount = credit,
                    CreatedAt = now
                });

                totalAgencyAmount += agencyAmount;
                totalGajAmount += gajAmount;
                totalCourseCount += quantity;

                // طرح ۸ فقط برای گزارش است.
                if (input.RegistrationPlanId == 8)
                    continue;

                totalDebit += debit;
                calculatedCredit += credit;
            }

            // اعتبار پرداخت‌های واقعی و اعمال‌شده؛
            // مستقل از آیتم‌های محاسباتی.
            var appliedPayments =
                await _store.GetAppliedSingleLessonPaymentsAsync(
                    settlement.Id, ct);

            settlement.TotalDebit = totalDebit;

            settlement.TotalCredit =
                calculatedCredit + appliedPayments;

            // با ثبت پرداخت هم‌راستا است؛
            // پرداخت بیشتر از بدهکاری مانده منفی ایجاد می‌کند.
            settlement.Balance =
                settlement.TotalDebit - settlement.TotalCredit;

            settlement.TotalAgencyAmount = totalAgencyAmount;
            settlement.TotalGajAmount = totalGajAmount;
            settlement.TotalCourseCount = totalCourseCount;

            if (!isNew)
                settlement.UpdatedAt = now;

            // آیتم‌ها به‌صورت گروهی جایگزین می‌شوند.
            await _store.ReplaceItemsAsync(settlement, items, ct);

            await _store.AddHistoryAsync(
                new SingleLessonSettlementHistory
                {
                    SettlementId = settlement.Id,
                    TotalDebit = settlement.TotalDebit,
                    TotalCredit = settlement.TotalCredit,
                    TotalDebitGaj = settlement.TotalDebitGaj,
                    TotalCreditGaj = settlement.TotalCreditGaj,
                    Balance = settlement.Balance,
                    TotalAgencyAmount = settlement.TotalAgencyAmount,
                    TotalGajAmount = settlement.TotalGajAmount,
                    BalanceGaj = settlement.BalanceGaj,
                    TotalCourseCount = settlement.TotalCourseCount,
                    Action = isNew ? "Created" : "Recalculated",
                    Description =
                        $"محاسبه خودکار تک‌درس؛ تعداد آیتم‌ها: {items.Count}",
                    CreatedAt = now
                },
                ct);


            await _store.SaveChangesAsync(ct);
        }
    }



    private static void ValidateConfiguration(
        SingleLessonPrice price,
        SingleLessonPercent percent,
        SingleLessonExternalInput input)
    {
        if (price.Amount < 0)
        {
            throw new InvalidOperationException(
                $"قیمت تک‌درس منفی است. PriceId={price.Id}.");
        }

        if (percent.AgencyPercent < 0 ||
            percent.GajPercent < 0 ||
            percent.AgencyPercent > 100 ||
            percent.GajPercent > 100 ||
            percent.AgencyPercent + percent.GajPercent != 100m)
        {
            throw new InvalidOperationException(
                $"مجموع درصدها باید ۱۰۰ باشد. PercentId={percent.Id}.");
        }

        if (!EligiblePlans.Contains(input.RegistrationPlanId))
        {
            throw new InvalidOperationException(
                "فقط طرح‌های ۱، ۲ و ۸ قابل محاسبه‌اند.");
        }
    }
}
