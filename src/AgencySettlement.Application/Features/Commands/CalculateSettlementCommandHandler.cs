
using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Settlements.Commands;
using AgencySettlement.Domain.Entities;
using MediatR;

namespace AgencySettlement.Application.Features.Settlements.Handlers;

public sealed class CalculateSettlementCommandHandler
    : IRequestHandler<CalculateSettlementCommand, SettlementResultDto>
{
    private readonly IExternalExamRecordRepository _externalRepository;
    private readonly IPriceRepository _priceRepository;
    private readonly IPercentRuleRepository _percentRepository;
    private readonly IAgencyRepository _agencyRepository;
    private readonly ISettlementRepository _settlementRepository;
    private readonly ISettlementHistoryRepository _historyRepository;

    public CalculateSettlementCommandHandler(
        IExternalExamRecordRepository externalRepository,
        IPriceRepository priceRepository,
        IPercentRuleRepository percentRepository,
        IAgencyRepository agencyRepository,
        ISettlementRepository settlementRepository,
        ISettlementHistoryRepository historyRepository)
    {
        _externalRepository = externalRepository;
        _priceRepository = priceRepository;
        _percentRepository = percentRepository;
        _agencyRepository = agencyRepository;
        _settlementRepository = settlementRepository;
        _historyRepository = historyRepository;
    }

    public async Task<SettlementResultDto> Handle(
        CalculateSettlementCommand command,
        CancellationToken cancellationToken)
    {
        ValidateRequest(command);

        var request = command.Request;

        var records = await _externalRepository
            .GetByAgencyAndYearAsync(
                request.AgencyId,
                request.YearId,
                request.PersianExecutionDate,
                cancellationToken);

        if (records.Count == 0)
        {
            throw new InvalidOperationException(
                "هیچ رکوردی برای این نماینده، سال و تاریخ پیدا نشد.");
        }

        var agency = await _agencyRepository.GetByIdAsync(
            request.AgencyId,
            cancellationToken);

        if (agency is null)
        {
            throw new InvalidOperationException(
                "نماینده پیدا نشد.");
        }

        var settlement = new Settlement
        {
            AgencyId = request.AgencyId,
            YearId = request.YearId,
            PersianExecutionDate = request.PersianExecutionDate,
            CreatedAt = DateTime.UtcNow
        };

        decimal totalDebit = 0m;
        decimal totalCredit = 0m;
        decimal totalCreditGaj = 0m;

        // اولویت مصرف سهمیه: اول ۵ (داوطلب آزاد) بعد ۳ (مدارس)
        var groups = records
            .GroupBy(x => new
            {
                x.PackageId,
                x.EducationalLevelId,
                x.ExamModeId,
                x.RegistrationPlanId,
                x.StudyFieldId,
                x.YearId,
                x.AgencyId,
                x.PersianExecutionDate
            })
            .OrderBy(x => GetQuotaPriority(x.Key.RegistrationPlanId));

        foreach (var group in groups)
        {
            var price = await _priceRepository.GetAsync(
                group.Key.PackageId,
                group.Key.EducationalLevelId,
                group.Key.ExamModeId,
                group.Key.RegistrationPlanId,
                group.Key.YearId,
                cancellationToken);

            if (price is null)
                continue;

            var candidateCount = group.Count();
            var planId = group.Key.RegistrationPlanId;
            var isQuotaPlan = IsFreeQuotaPlan(planId);

            var isAlwaysFree =
                isQuotaPlan &&
                group.Key.ExamModeId == 1;

            var freeQuotaCount = 0;

            if (isQuotaPlan && !isAlwaysFree)
            {
                freeQuotaCount =
                    await _agencyRepository.ConsumeFreeQuotaAsync(
                        request.AgencyId,
                        candidateCount,
                        cancellationToken);
            }
            else if (isAlwaysFree)
            {
                freeQuotaCount = candidateCount;
            }

            var paidCandidateCount =
                candidateCount - freeQuotaCount;

            var unitPrice = price.Amount;
            var baseAmount = candidateCount * unitPrice;
            var paidBaseAmount =
                paidCandidateCount * unitPrice;

            decimal gajAmount = 0m;
            decimal agencyAmount = 0m;
            decimal studentAmount = 0m;
            decimal debitAmount = 0m;
            decimal creditAmount = 0m;
            decimal agencyPercent = 0m;
            decimal gajPercent = 0m;
            decimal studentPercent = 0m;

            if (isQuotaPlan)
            {
                gajAmount = paidBaseAmount;
                agencyAmount = paidBaseAmount;
                studentAmount = 0m;

                debitAmount = paidBaseAmount;
                creditAmount = 0m;

                totalDebit += debitAmount;
            }
            else if (planId == 2)
            {
                gajAmount = baseAmount;
                agencyAmount = baseAmount;
                studentAmount = 0m;

                debitAmount = 0m;
                creditAmount = baseAmount;

                totalCredit += creditAmount;
            }
            else
            {
                var percent = await _percentRepository.GetAsync(
                    request.AgencyId,
                    group.Key.ExamModeId,
                    cancellationToken);

                if (percent is null)
                    continue;

                agencyPercent = percent.AgencyPercent;
                gajPercent = percent.GajPercent;
                studentPercent = percent.StudentPercent;

                gajAmount =
                    CalculateShare(
                        baseAmount,
                        percent.AgencyPercent);

                agencyAmount =
                    CalculateShare(
                        baseAmount,
                        percent.GajPercent);

                studentAmount =
                    CalculateShare(
                        baseAmount,
                        percent.StudentPercent);

                if (planId == 1)
                {
                    debitAmount = agencyAmount;
                    creditAmount = 0m;

                    totalDebit += debitAmount;
                    totalCreditGaj += gajAmount;
                }
                else if (planId == 8)
                {
                    debitAmount = 0m;
                    creditAmount = agencyAmount;

                    totalCredit += creditAmount;
                }
            }

            settlement.Items.Add(
                new SettlementItem
                {
                    PackageId = group.Key.PackageId,
                    EducationalLevelId =
                        group.Key.EducationalLevelId,
                    ExamModeId = group.Key.ExamModeId,
                    RegistrationPlanId =
                        group.Key.RegistrationPlanId,
                    YearId = group.Key.YearId,
                    StudyFieldId = group.Key.StudyFieldId,
                    PersianExecutionDate =
                        group.Key.PersianExecutionDate,
                    AgencyId = group.Key.AgencyId,
                    CandidateCount = candidateCount,
                    FreeCandidateCount = freeQuotaCount,
                    PaidCandidateCount = paidCandidateCount,
                    UnitPrice = unitPrice,
                    BaseAmount = baseAmount,
                    AgencyPercent = agencyPercent,
                    GajPercent = gajPercent,
                    StudentPercent = studentPercent,
                    AgencyAmount = gajAmount,
                    GajAmount = agencyAmount,
                    StudentAmount = studentAmount,
                    DebitAmount = debitAmount,
                    CreditAmount = creditAmount,
                    CreatedAt = DateTime.UtcNow
                });
        }

        if (settlement.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
        }

        settlement.TotalDebit =
            Math.Max(0m, totalDebit);

        settlement.TotalCredit =
            Math.Max(0m, totalCredit);

        settlement.Balance =
            settlement.TotalDebit -
            settlement.TotalCredit;

        settlement.TotalDebitGaj =
            settlement.TotalCredit;

        settlement.TotalCreditGaj =
            totalCreditGaj;

        settlement.BalanceGaj =
            Math.Max(
                0m,
                settlement.TotalCreditGaj -
                settlement.TotalDebit);

        await _settlementRepository.AddAsync(
            settlement,
            cancellationToken);

        await _settlementRepository.SaveChangesAsync(
            cancellationToken);

        var history = new SettlementHistory
        {
            SettlementId = settlement.Id,
            AgencyId = settlement.AgencyId,
            YearId = settlement.YearId,
            PersianExecutionDate =
                settlement.PersianExecutionDate,
            TotalDebit = settlement.TotalDebit,
            TotalCredit = settlement.TotalCredit,
            Balance = settlement.Balance,
            TotalDebitGaj = settlement.TotalDebitGaj,
            TotalCreditGaj = settlement.TotalCreditGaj,
            BalanceGaj = settlement.BalanceGaj,
            CreatedAt = DateTime.UtcNow,
            Description = "محاسبه Settlement نماینده"
        };

        await _historyRepository.AddAsync(
            history,
            cancellationToken);

        await _settlementRepository.SaveChangesAsync(
            cancellationToken);

        return CreateResult(settlement);
    }

    private static bool IsFreeQuotaPlan(
        int registrationPlanId)
        => registrationPlanId is 3 or 5;

    private static int GetQuotaPriority(
        int registrationPlanId)
        => registrationPlanId switch
        {
            5 => 1,
            3 => 2,
            _ => 4
        };

    private static decimal CalculateShare(
        decimal baseAmount,
        decimal percent)
        => baseAmount * percent / 100m;

    private static void ValidateRequest(
        
        CalculateSettlementCommand command)
    {
        if (command.Request.AgencyId <= 0)
            throw new ArgumentException(
                "AgencyId نامعتبر است.");

        if (command.Request.YearId <= 0)
            throw new ArgumentException(
                "YearId نامعتبر است.");

        if (string.IsNullOrWhiteSpace(
            command.Request.PersianExecutionDate))
        {
            throw new ArgumentException(
                "PersianExecutionDate الزامی است.");
        }
    }

    private static SettlementResultDto CreateResult(
        Settlement settlement)
    {
        return new SettlementResultDto
        {
            SettlementId = settlement.Id,
            AgencyId = settlement.AgencyId,
            TotalDebit = settlement.TotalDebit,
            TotalCredit = settlement.TotalCredit,
            Balance = settlement.Balance,
            TotalDebitGaj = settlement.TotalDebitGaj,
            TotalCreditGaj = settlement.TotalCreditGaj,
            BalanceGaj = settlement.BalanceGaj,
            PersianExecutionDate =
                settlement.PersianExecutionDate,

            Items = settlement.Items
                .Select(x => new SettlementItemResultDto
                {
                    PackageId = x.PackageId,
                    EducationalLevelId =
                        x.EducationalLevelId,
                    StudyFieldId = x.StudyFieldId,
                    ExamModeId = x.ExamModeId,
                    RegistrationPlanId =
                        x.RegistrationPlanId,
                    YearId = x.YearId,
                    CandidateCount = x.CandidateCount,
                    FreeCandidateCount =
                        x.FreeCandidateCount,
                    PaidCandidateCount =
                        x.PaidCandidateCount,
                    UnitPrice = x.UnitPrice,
                    BaseAmount = x.BaseAmount,
                    AgencyPercent = x.AgencyPercent,
                    GajPercent = x.GajPercent,
                    StudentPercent = x.StudentPercent,
                    AgencyAmount = x.AgencyAmount,
                    GajAmount = x.GajAmount,
                    StudentAmount = x.StudentAmount,
                    DebitAmount = x.DebitAmount,
                    CreditAmount = x.CreditAmount,
                    PersianExecutionDate =
                        x.PersianExecutionDate
                })
                .ToList()
        };
    }
}




//using AgencySettlement.Application.Abstractions.External;
//using AgencySettlement.Application.Abstractions.Persistence.Repositories;
//using AgencySettlement.Application.Settlements.Commands;
//using AgencySettlement.Domain.Entities;
//using MediatR;

//namespace AgencySettlement.Application.Features.Settlements.Handlers;


//public sealed class CalculateSettlementCommandHandler
//    : IRequestHandler<CalculateSettlementCommand, SettlementResultDto>
//{
//    private readonly IExternalExamRecordRepository _externalRepository;
//    private readonly IPriceRepository _priceRepository;
//    private readonly IPercentRuleRepository _percentRepository;
//    private readonly IAgencyRepository _agencyRepository;
//    private readonly ISettlementRepository _settlementRepository;
//    private readonly ISettlementHistoryRepository _historyRepository;

//    public CalculateSettlementCommandHandler(
//        IExternalExamRecordRepository externalRepository,
//        IPriceRepository priceRepository,
//        IPercentRuleRepository percentRepository,
//        IAgencyRepository agencyRepository,
//        ISettlementRepository settlementRepository,
//        ISettlementHistoryRepository historyRepository)
//    {
//        _externalRepository = externalRepository;
//        _priceRepository = priceRepository;
//        _percentRepository = percentRepository;
//        _agencyRepository = agencyRepository;
//        _settlementRepository = settlementRepository;
//        _historyRepository = historyRepository;
//    }

//    public async Task<SettlementResultDto> Handle(
//        CalculateSettlementCommand command,
//        CancellationToken cancellationToken)
//    {
//        ValidateRequest(command);

//        var request = command.Request;

//        var records = await _externalRepository
//            .GetByAgencyAndYearAsync(
//                request.AgencyId,
//                request.YearId,
//                cancellationToken);

//        if (records.Count == 0)
//        {
//            throw new InvalidOperationException(
//                "هیچ رکوردی برای این نماینده و سال پیدا نشد.");
//        }

//        var agency = await _agencyRepository.GetByIdAsync(
//            request.AgencyId,
//            cancellationToken);

//        if (agency is null)
//        {
//            throw new InvalidOperationException(
//                "نماینده پیدا نشد.");
//        }

//        var settlement = new Settlement
//        {
//            AgencyId = request.AgencyId,
//            YearId = request.YearId,
//            CreatedAt = DateTime.UtcNow
//        };

//        decimal totalDebit = 0m;
//        decimal totalCredit = 0m;
//        decimal totalCreditGaj = 0m;

//        // اولویت مصرف سهمیه: اول ۵ (داوطلب آزاد) بعد ۳ (مدارس)
//        var groups = records
//            .GroupBy(x => new
//            {
//                x.PackageId,
//                x.EducationalLevelId,
//                x.ExamModeId,
//                x.RegistrationPlanId,
//                x.StudyFieldId,
//                x.YearId,
//                x.AgencyId,
//                x.PersianExecutionDate
//            })
//            .OrderBy(x => GetQuotaPriority(x.Key.RegistrationPlanId));

//        foreach (var group in groups)
//        {
//            var price = await _priceRepository.GetAsync(
//                group.Key.PackageId,
//                group.Key.EducationalLevelId,
//                group.Key.ExamModeId,
//                group.Key.RegistrationPlanId,
//                group.Key.YearId,
//                cancellationToken);

//            if (price is null)
//                continue;

//            var candidateCount = group.Count();
//            var planId = group.Key.RegistrationPlanId;
//            var isQuotaPlan = IsFreeQuotaPlan(planId); // فقط ۳ و ۵

//            // فقط وقتی RegistrationPlanId برابر ۳ یا ۵ و ExamModeId = 1 باشد → همیشه رایگان
//            var isAlwaysFree = isQuotaPlan && group.Key.ExamModeId == 1;

//            // -------------------------------------------------
//            // سهمیه رایگان فقط و فقط برای RegistrationPlanId = 3 و 5
//            // -------------------------------------------------
//            var freeQuotaCount = 0;
//            if (isQuotaPlan && !isAlwaysFree)
//            {
//                freeQuotaCount = await _agencyRepository.ConsumeFreeQuotaAsync(
//                    request.AgencyId,
//                    candidateCount,
//                    cancellationToken);
//            }
//            else if (isAlwaysFree)
//            {
//                freeQuotaCount = candidateCount;
//            }

//            var paidCandidateCount = candidateCount - freeQuotaCount;

//            var unitPrice = price.Amount;
//            var baseAmount = candidateCount * unitPrice;
//            var paidBaseAmount = paidCandidateCount * unitPrice;

//            decimal gajAmount = 0m;
//            decimal agencyAmount = 0m;
//            decimal studentAmount = 0m;
//            decimal debitAmount = 0m;
//            decimal creditAmount = 0m;
//            decimal agencyPercent = 0m;
//            decimal gajPercent = 0m;
//            decimal studentPercent = 0m;

//            if (isQuotaPlan)
//            {
//                // -------------------------------------------------
//                // طرح ۳ و ۵ (بورسیه)
//                // - قیمت ثابت
//                // - فقط TotalDebit (بدهکار)
//                // - هیچ‌وقت به TotalCreditGaj اضافه نمی‌شود
//                // -------------------------------------------------
//                gajAmount = paidBaseAmount;
//                agencyAmount = paidBaseAmount;
//                studentAmount = 0m;

//                debitAmount = paidBaseAmount;
//                creditAmount = 0m;

//                totalDebit += debitAmount;
//            }
//            else if (planId == 2)
//            {
//                // -------------------------------------------------
//                // طرح ۲ (حکمت)
//                // - قیمت ثابت (درصد ندارد)
//                // - فقط TotalCredit (بستانکار)
//                // - هیچ‌وقت به TotalCreditGaj اضافه نمی‌شود
//                // -------------------------------------------------
//                gajAmount = baseAmount;
//                agencyAmount = baseAmount;
//                studentAmount = 0m;

//                debitAmount = 0m;
//                creditAmount = baseAmount;

//                totalCredit += creditAmount;
//            }
//            else
//            {
//                // -------------------------------------------------
//                // طرح ۱ و ۸ → منطق درصد
//                // طرح ۱: بدهکار + TotalCreditGaj
//                // طرح ۸: دقیقاً مثل ۱ ولی بستانکار (TotalCredit) و بدون TotalCreditGaj
//                // -------------------------------------------------
//                var percent = await _percentRepository.GetAsync(
//                    request.AgencyId,
//                    group.Key.ExamModeId,
//                    cancellationToken);

//                if (percent is null)
//                    continue;

//                agencyPercent = percent.AgencyPercent;
//                gajPercent = percent.GajPercent;
//                studentPercent = percent.StudentPercent;

//                // درصد بالاتر = سهم گاج
//                gajAmount = CalculateShare(baseAmount, percent.AgencyPercent);

//                // درصد پایین‌تر = سهم نماینده
//                agencyAmount = CalculateShare(baseAmount, percent.GajPercent);

//                studentAmount = CalculateShare(baseAmount, percent.StudentPercent);

//                if (planId == 1)
//                {
//                    // آزاد → بدهکار + سهم گاج
//                    debitAmount = agencyAmount;
//                    creditAmount = 0m;

//                    totalDebit += debitAmount;
//                    totalCreditGaj += gajAmount;
//                }
//                else if (planId == 8)
//                {
//                    // ثبت‌نام از سایت → دقیقاً مثل ۱ ولی بستانکار
//                    debitAmount = 0m;
//                    creditAmount = agencyAmount;

//                    totalCredit += creditAmount;
//                    // عمداً totalCreditGaj اضافه نمی‌شود
//                }
//            }

//            // ---------------------------------------------
//            // Settlement Item
//            // ---------------------------------------------
//            settlement.Items.Add(
//                new SettlementItem
//                {
//                    PackageId = group.Key.PackageId,
//                    EducationalLevelId = group.Key.EducationalLevelId,
//                    ExamModeId = group.Key.ExamModeId,
//                    RegistrationPlanId = group.Key.RegistrationPlanId,
//                    YearId = group.Key.YearId,
//                    StudyFieldId = group.Key.StudyFieldId,
//                    PersianExecutionDate = group.Key.PersianExecutionDate,
//                    AgencyId = group.Key.AgencyId,
//                    CandidateCount = candidateCount,
//                    FreeCandidateCount = freeQuotaCount,
//                    PaidCandidateCount = paidCandidateCount,
//                    UnitPrice = unitPrice,
//                    BaseAmount = baseAmount,
//                    AgencyPercent = agencyPercent,
//                    GajPercent = gajPercent,
//                    StudentPercent = studentPercent,
//                    AgencyAmount = gajAmount,
//                    GajAmount = agencyAmount,
//                    StudentAmount = studentAmount,
//                    DebitAmount = debitAmount,
//                    CreditAmount = creditAmount,
//                    CreatedAt = DateTime.UtcNow
//                });
//        }

//        if (settlement.Items.Count == 0)
//        {
//            throw new InvalidOperationException(
//                "هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
//        }

//        // ---------------------------------------------
//        // مبلغ نهایی نماینده
//        // ---------------------------------------------
//        settlement.TotalDebit = Math.Max(0m, totalDebit);
//        settlement.TotalCredit = Math.Max(0m, totalCredit);
//        settlement.Balance = settlement.TotalDebit - settlement.TotalCredit;

//        // ---------------------------------------------
//        // حساب گاج
//        // ---------------------------------------------
//        settlement.TotalDebitGaj = settlement.TotalCredit;
//        settlement.TotalCreditGaj = totalCreditGaj;
//        settlement.BalanceGaj = Math.Max(0m, settlement.TotalCreditGaj - settlement.TotalDebit);

//        // ---------------------------------------------
//        // تاریخ اجرا
//        // ---------------------------------------------
//        settlement.PersianExecutionDate =
//            settlement.Items
//                .Select(x => x.PersianExecutionDate)
//                .FirstOrDefault()
//            ?? string.Empty;

//        // ---------------------------------------------
//        // ذخیره Settlement
//        // ---------------------------------------------
//        await _settlementRepository.AddAsync(settlement, cancellationToken);
//        await _settlementRepository.SaveChangesAsync(cancellationToken);

//        // ---------------------------------------------
//        // History
//        // ---------------------------------------------
//        var history = new SettlementHistory
//        {
//            SettlementId = settlement.Id,
//            AgencyId = settlement.AgencyId,
//            YearId = settlement.YearId,
//            PersianExecutionDate = settlement.PersianExecutionDate,
//            TotalDebit = settlement.TotalDebit,
//            TotalCredit = settlement.TotalCredit,
//            Balance = settlement.Balance,
//            TotalDebitGaj = settlement.TotalDebitGaj,
//            TotalCreditGaj = settlement.TotalCreditGaj,
//            BalanceGaj = settlement.BalanceGaj,
//            CreatedAt = DateTime.UtcNow,
//            Description = "محاسبه Settlement نماینده"
//        };

//        await _historyRepository.AddAsync(history, cancellationToken);
//        await _settlementRepository.SaveChangesAsync(cancellationToken);

//        return CreateResult(settlement);
//    }

//    /// <summary>
//    /// سهمیه رایگان فقط مختص طرح‌های ۳ و ۵ است.
//    /// </summary>
//    private static bool IsFreeQuotaPlan(int registrationPlanId)
//        => registrationPlanId is 3 or 5;

//    /// <summary>
//    /// اولویت مصرف سهمیه: اول ۵ بعد ۳
//    /// </summary>
//    private static int GetQuotaPriority(int registrationPlanId)
//        => registrationPlanId switch
//        {
//            5 => 1,
//            3 => 2,
//            _ => 4
//        };

//    private static decimal CalculateShare(decimal baseAmount, decimal percent)
//        => baseAmount * percent / 100m;

//    private static void ValidateRequest(CalculateSettlementCommand command)
//    {
//        if (command.Request.AgencyId <= 0)
//            throw new ArgumentException("AgencyId نامعتبر است.");

//        if (command.Request.YearId <= 0)
//            throw new ArgumentException("YearId نامعتبر است.");
//    }

//    private static SettlementResultDto CreateResult(Settlement settlement)
//    {
//        return new SettlementResultDto
//        {
//            SettlementId = settlement.Id,
//            AgencyId = settlement.AgencyId,
//            TotalDebit = settlement.TotalDebit,
//            TotalCredit = settlement.TotalCredit,
//            Balance = settlement.Balance,
//            TotalDebitGaj = settlement.TotalDebitGaj,
//            TotalCreditGaj = settlement.TotalCreditGaj,
//            BalanceGaj = settlement.BalanceGaj,
//            PersianExecutionDate = settlement.PersianExecutionDate,
//            Items = settlement.Items
//                .Select(x => new SettlementItemResultDto
//                {
//                    PackageId = x.PackageId,
//                    EducationalLevelId = x.EducationalLevelId,
//                    StudyFieldId = x.StudyFieldId,
//                    ExamModeId = x.ExamModeId,
//                    RegistrationPlanId = x.RegistrationPlanId,
//                    YearId = x.YearId,
//                    CandidateCount = x.CandidateCount,
//                    FreeCandidateCount = x.FreeCandidateCount,
//                    PaidCandidateCount = x.PaidCandidateCount,
//                    UnitPrice = x.UnitPrice,
//                    BaseAmount = x.BaseAmount,
//                    AgencyPercent = x.AgencyPercent,
//                    GajPercent = x.GajPercent,
//                    StudentPercent = x.StudentPercent,
//                    AgencyAmount = x.AgencyAmount,
//                    GajAmount = x.GajAmount,
//                    StudentAmount = x.StudentAmount,
//                    DebitAmount = x.DebitAmount,
//                    CreditAmount = x.CreditAmount,
//                    PersianExecutionDate = x.PersianExecutionDate
//                })
//                .ToList()
//        };
//    }
//}













