using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Domain.Entities.SingleLesson;
using AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons.Rules;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons;

public interface ISingleLessonSettlementCoordinator
{
    Task RecalculateAsync(IEnumerable<SingleLessonSettlementGroup> groups, CancellationToken ct);
}

public sealed class SingleLessonSettlementCoordinator : ISingleLessonSettlementCoordinator
{
    private static readonly HashSet<int> EligiblePlans = [1, 2, 8];
    private readonly ISingleLessonSettlementStore _store;
    private readonly ISingleLessonPlanRuleFactory _ruleFactory;

    public SingleLessonSettlementCoordinator(ISingleLessonSettlementStore store, ISingleLessonPlanRuleFactory ruleFactory) { _store = store; _ruleFactory = ruleFactory; }

    //public async Task RecalculateAsync(IEnumerable<SingleLessonSettlementGroup> groups, CancellationToken ct)
    //{
    //    var distinct = groups.Where(x => x.AgencyId > 0 && x.YearId > 0 && !string.IsNullOrWhiteSpace(x.PersianExecutionDate))
    //        .Distinct().ToList();
    //    foreach (var key in distinct)
    //    {



    //        var inputs = (await _store.GetInputsAsync(key.AgencyId, key.YearId, key.PersianExecutionDate, ct))
    //            .Where(x => EligiblePlans.Contains(x.RegistrationPlanId)).ToList();

    //        var items = new List<SingleLessonSettlementItem>(inputs.Count);
    //        foreach (var input in inputs)
    //        {


    //            var price = await _store.GetPriceAsync(input, ct)
    //?? throw new InvalidOperationException(
    //    $"قیمت فعال تک‌درس پیدا نشد: " +
    //    $"CandidateExamId={input.CandidateExamId}, " +
    //    $"ExternalInputId={input.Id}, " +
    //    $"CourseId={input.CourseId}, " +
    //    $"YearId={input.YearId}, " +
    //    $"PackageId={input.PackageId}, " +
    //    $"StudyFieldId={input.StudyFieldId}, " +
    //    $"EducationalLevelId={input.EducationalLevelId}.");

    //            var percent = await _store.GetPercentAsync(input, ct)
    //                ?? throw new InvalidOperationException($"درصد فعال تک‌درس پیدا نشد: CourseId={input.CourseId}, YearId={input.YearId}, AgencyId={input.AgencyId}.");
    //            ValidateConfiguration(price, percent, input);
    //            const int quantity = 1;
    //            var baseAmount = decimal.Round(price.Amount * quantity, 2, MidpointRounding.AwayFromZero);
    //            var agencyAmount = decimal.Round(baseAmount * percent.AgencyPercent / 100m, 2, MidpointRounding.AwayFromZero);

    //            var gajAmount = baseAmount - agencyAmount;
    //            var (debit, credit) = _ruleFactory.Get(input.RegistrationPlanId).Calculate(agencyAmount);
    //            items.Add(new SingleLessonSettlementItem {
    //                ExternalInputId = input.Id, AgencyId = input.AgencyId, CourseId = input.CourseId,
    //                YearId = input.YearId, PersianExecutionDate = input.PersianExecutionDate,
    //                PackageId = input.PackageId, EducationalLevelId = input.EducationalLevelId,
    //                ExamModeId = input.ExamModeId, StudyFieldId = input.StudyFieldId,
    //                RegistrationPlanId = input.RegistrationPlanId, Quantity = quantity,
    //                UnitPrice = price.Amount, BaseAmount = baseAmount,
    //                AgencyPercent = percent.AgencyPercent, GajPercent = percent.GajPercent,
    //                AgencyAmount = agencyAmount, GajAmount = gajAmount,
    //                DebitAmount = debit, CreditAmount = credit, CreatedAt = DateTime.UtcNow
    //            });
    //        }
    //        var settlement = await _store.GetSettlementAsync(key.AgencyId, key.YearId, key.PersianExecutionDate, ct);
    //        var isNew = settlement is null;

    //        // If a record moved out of an old group, do not create a meaningless empty settlement.
    //        // If that group already has a settlement, clear its items and zero its totals below.
    //        if (isNew && items.Count == 0)
    //            continue;

    //        settlement ??= new SingleLessonSettlement { AgencyId = key.AgencyId, YearId = key.YearId, PersianExecutionDate = key.PersianExecutionDate, CreatedAt = DateTime.UtcNow };




    //        // settlement.TotalDebit = items.Sum(x => x.DebitAmount);
    //        // settlement.TotalCredit = items.Sum(x => x.CreditAmount);

    //        //settlement.Balance = settlement.TotalCredit - settlement.TotalDebit;

    //        //  settlement.Balance = Math.Max(
    //        //  0m,
    //        //  settlement.TotalDebit - settlement.TotalCredit);


    //        // آیتم‌ها و مبلغ اصلی‌شان حفظ می‌شوند.
    //        // فقط آیتم‌های غیرطرح ۸ در جمع مالی عمومی شرکت می‌کنند.
    //        var financialItems = items
    //            .Where(x => x.RegistrationPlanId != 8)
    //            .ToList();

    //        settlement.TotalDebit =
    //            financialItems.Sum(x => x.DebitAmount);

    //        settlement.TotalCredit =
    //            financialItems.Sum(x => x.CreditAmount);

    //        settlement.Balance = Math.Max(
    //            0m,
    //            settlement.TotalDebit - settlement.TotalCredit);



    //        settlement.TotalAgencyAmount = items.Sum(x => x.AgencyAmount);
    //        settlement.TotalGajAmount = items.Sum(x => x.GajAmount);
    //        settlement.TotalCourseCount = items.Sum(x => x.Quantity);
    //        settlement.UpdatedAt = isNew ? null : DateTime.UtcNow;
    //        if (isNew) await _store.AddSettlementAsync(settlement, ct);
    //        await _store.SaveChangesAsync(ct); // ensures new settlement has its BIGINT identity before items are attached
    //        await _store.ReplaceItemsAsync(settlement, items, ct);
    //        await _store.AddHistoryAsync(

    //            new SingleLessonSettlementHistory {


    //            SettlementId = settlement.Id, 
    //                TotalDebit = settlement.TotalDebit,
    //                TotalCredit = settlement.TotalCredit,
    //                TotalDebitGaj = settlement.TotalDebitGaj,
    //                TotalCreditGaj = settlement.TotalCreditGaj,
    //                BalanceGaj = settlement.BalanceGaj,
    //                TotalCourseCount = settlement.TotalCourseCount,
    //            Action = isNew ? "Created" : "Recalculated", 
    //                Description = $"محاسبه خودکار تک‌درس؛ تعداد آیتم‌های قابل تسویه: {items.Count}",
    //                CreatedAt = DateTime.UtcNow
    //        }, ct);
    //        await _store.SaveChangesAsync(ct);
    //    }
    //}



    public async Task RecalculateAsync(
    IEnumerable<SingleLessonSettlementGroup> groups,
    CancellationToken ct)
    {
        var distinctGroups = groups
            .Where(x =>
                x.AgencyId > 0 &&
                x.YearId > 0 &&
                !string.IsNullOrWhiteSpace(x.PersianExecutionDate))
            .Distinct()
            .ToList();

        // Cache فقط در طول همین اجرای محاسبه زنده است.
        var priceCache =
            new Dictionary<
                (int CourseId, int YearId, int PackageId, int StudyFieldId, int EducationalLevelId),
                SingleLessonPrice?>();

        var percentCache =
            new Dictionary<
                (int CourseId, int YearId, int PackageId, int StudyFieldId, int EducationalLevelId, int AgencyId),
                SingleLessonPercent?>();

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

            var items = new List<SingleLessonSettlementItem>(inputs.Count);

            foreach (var input in inputs)
            {
                ct.ThrowIfCancellationRequested();

                var priceKey = (
                    input.CourseId,
                    input.YearId,
                    input.PackageId,
                    input.StudyFieldId,
                    input.EducationalLevelId);

                if (!priceCache.TryGetValue(priceKey, out var price))
                {
                    price = await _store.GetPriceAsync(input, ct);
                    priceCache.Add(priceKey, price);
                }

                if (price is null)
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

                var percentKey = (
                    input.CourseId,
                    input.YearId,
                    input.PackageId,
                    input.StudyFieldId,
                    input.EducationalLevelId,
                    input.AgencyId);

                if (!percentCache.TryGetValue(percentKey, out var percent))
                {
                    percent = await _store.GetPercentAsync(input, ct);
                    percentCache.Add(percentKey, percent);
                }

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
                    CreatedAt = DateTime.UtcNow
                });
            }

            var settlement = await _store.GetSettlementAsync(
                key.AgencyId,
                key.YearId,
                key.PersianExecutionDate,
                ct);

            var isNew = settlement is null;

            if (isNew && items.Count == 0)
                continue;

            settlement ??= new SingleLessonSettlement
            {
                AgencyId = key.AgencyId,
                YearId = key.YearId,
                PersianExecutionDate = key.PersianExecutionDate,
                CreatedAt = DateTime.UtcNow
            };

            // منطق مالی بدون تغییر:
            // پلن ۸ در TotalDebit و TotalCredit شرکت نمی‌کند.
            decimal totalDebit = 0m;
            decimal totalCredit = 0m;

            foreach (var item in items)
            {
                if (item.RegistrationPlanId == 8)
                    continue;

                totalDebit += item.DebitAmount;
                totalCredit += item.CreditAmount;
            }

            settlement.TotalDebit = totalDebit;
            settlement.TotalCredit = totalCredit;

            settlement.Balance = Math.Max(
                0m,
                settlement.TotalDebit - settlement.TotalCredit);

            settlement.TotalAgencyAmount =
                items.Sum(x => x.AgencyAmount);

            settlement.TotalGajAmount =
                items.Sum(x => x.GajAmount);

            settlement.TotalCourseCount =
                items.Sum(x => x.Quantity);

            settlement.UpdatedAt = isNew ? null : DateTime.UtcNow;

            if (isNew)
            {
                await _store.AddSettlementAsync(settlement, ct);

                // برای تولید Id رکورد جدید ضروری است.
                await _store.SaveChangesAsync(ct);
            }

            await _store.ReplaceItemsAsync(settlement, items, ct);

            await _store.AddHistoryAsync(
                new SingleLessonSettlementHistory
                {
                    SettlementId = settlement.Id,
                    TotalDebit = settlement.TotalDebit,
                    TotalCredit = settlement.TotalCredit,
                    TotalDebitGaj = settlement.TotalDebitGaj,
                    TotalCreditGaj = settlement.TotalCreditGaj,
                    BalanceGaj = settlement.BalanceGaj,
                    TotalCourseCount = settlement.TotalCourseCount,
                    Action = isNew ? "Created" : "Recalculated",
                    Description =
                        $"محاسبه خودکار تک‌درس؛ تعداد آیتم‌های قابل تسویه: {items.Count}",
                    CreatedAt = DateTime.UtcNow
                },
                ct);

            await _store.SaveChangesAsync(ct);
        }
    }

    private static void ValidateConfiguration(SingleLessonPrice price, SingleLessonPercent percent, SingleLessonExternalInput input)
    {
        if (price.Amount < 0) throw new InvalidOperationException($"قیمت تک‌درس منفی است. PriceId={price.Id}.");
        if (percent.AgencyPercent < 0 || percent.GajPercent < 0 || percent.AgencyPercent > 100 || percent.GajPercent > 100 || percent.AgencyPercent + percent.GajPercent != 100m)
            throw new InvalidOperationException($"درصدهای تک‌درس باید غیرمنفی باشند و مجموعشان ۱۰۰ شود. PercentId={percent.Id}.");
        if (input.RegistrationPlanId is not (1 or 2 or 8)) throw new InvalidOperationException("فقط RegistrationPlanIdهای 1، 2 و 8 قابل محاسبه‌اند.");
    }
}
