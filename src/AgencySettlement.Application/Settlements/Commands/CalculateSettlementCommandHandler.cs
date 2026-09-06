
using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Domain.Entities;
using MediatR;

namespace AgencySettlement.Application.Settlements.Commands;

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
                cancellationToken);

        if (records.Count == 0)
        {
            throw new InvalidOperationException(
                "هیچ رکوردی برای این نماینده و سال پیدا نشد.");
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
            CreatedAt = DateTime.UtcNow
        };

        var grossTotalDebit = 0m;
        var grossTotalCredit = 0m;

        var totalDebitQuotaAmount = 0m;
        var totalCreditQuotaAmount = 0m;

        var groups = records
            .GroupBy(x => new
            {
                x.PackageId,
                x.EducationalLevelId,
                x.ExamModeId,
                x.RegistrationPlanId,
                x.YearId
            })
            .OrderBy(x =>
                GetQuotaPriority(
                    x.Key.RegistrationPlanId));

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

            var percent = await _percentRepository.GetAsync(
                request.AgencyId,
                group.Key.ExamModeId,
                cancellationToken);

            if (percent is null)
                continue;

            var candidateCount = group.Count();

            var quotaCount = 0;

            if (IsFreeQuotaPlan(group.Key.RegistrationPlanId))
            {
                quotaCount =
                    await _agencyRepository.ConsumeFreeQuotaAsync(
                        request.AgencyId,
                        candidateCount,
                        cancellationToken);
            }

            var baseAmount =
                candidateCount * price.Amount;

            var agencyAmount =
                CalculateShare(
                    baseAmount,
                    percent.AgencyPercent);

            var gajAmount =
                CalculateShare(
                    baseAmount,
                    percent.GajPercent);

            var studentAmount =
                CalculateShare(
                    baseAmount,
                    percent.StudentPercent);

            var (debitAmount, creditAmount) =
                CalculateSettlementAmount(
                    group.Key.RegistrationPlanId,
                    gajAmount);

            grossTotalDebit += debitAmount;
            grossTotalCredit += creditAmount;

            var quotaAmount =
                quotaCount * price.Amount;

            if (group.Key.RegistrationPlanId is 3 or 5)
            {
                totalDebitQuotaAmount += quotaAmount;
            }
            else if (group.Key.RegistrationPlanId == 8)
            {
                totalCreditQuotaAmount += quotaAmount;
            }

            settlement.Items.Add(
                new SettlementItem
                {
                    PackageId =
                        group.Key.PackageId,

                    EducationalLevelId =
                        group.Key.EducationalLevelId,

                    ExamModeId =
                        group.Key.ExamModeId,

                    RegistrationPlanId =
                        group.Key.RegistrationPlanId,

                    YearId =
                        group.Key.YearId,

                    CandidateCount =
                        candidateCount,

                    FreeCandidateCount =
                        quotaCount,

                    PaidCandidateCount =
                        candidateCount,

                    UnitPrice =
                        price.Amount,

                    BaseAmount =
                        baseAmount,

                    AgencyPercent =
                        percent.AgencyPercent,

                    GajPercent =
                        percent.GajPercent,

                    StudentPercent =
                        percent.StudentPercent,

                    AgencyAmount =
                        agencyAmount,

                    GajAmount =
                        gajAmount,

                    StudentAmount =
                        studentAmount,

                    DebitAmount =
                        debitAmount,

                    CreditAmount =
                        creditAmount,

                    CreatedAt =
                        DateTime.UtcNow
                });
        }

        if (settlement.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
        }

        var totalDebit =
            grossTotalDebit - totalDebitQuotaAmount;

        var totalCredit =
            grossTotalCredit - totalCreditQuotaAmount;

        settlement.TotalDebit =
            Math.Max(0m, totalDebit);

        settlement.TotalCredit =
            Math.Max(0m, totalCredit);

        settlement.Balance =
            settlement.TotalDebit -
            settlement.TotalCredit;

        await _settlementRepository.AddAsync(
            settlement,
            cancellationToken);

        await _settlementRepository.SaveChangesAsync(
            cancellationToken);

        var history = new SettlementHistory
        {
            SettlementId =
                settlement.Id,

            AgencyId =
                settlement.AgencyId,

            TotalDebit =
                settlement.TotalDebit,

            TotalCredit =
                settlement.TotalCredit,

            Balance =
                settlement.Balance,

            CreatedAt =
                DateTime.UtcNow,

            Description =
                "محاسبه Settlement نماینده"
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
    {
        return registrationPlanId is 3 or 5 or 8;
    }

    private static int GetQuotaPriority(
        int registrationPlanId)
    {
        return registrationPlanId switch
        {
            3 => 1,
            5 => 2,
            8 => 3,
            _ => 4
        };
    }

    private static decimal CalculateShare(
        decimal baseAmount,
        decimal percent)
    {
        return baseAmount * percent / 100m;
    }

    private static (decimal Debit, decimal Credit)
        CalculateSettlementAmount(
            int registrationPlanId,
            decimal gajAmount)
    {
        return registrationPlanId switch
        {
            1 => (gajAmount, 0m),
            3 => (gajAmount, 0m),
            5 => (gajAmount, 0m),

            2 => (0m, gajAmount),
            8 => (0m, gajAmount),

            _ => (0m, 0m)
        };
    }

    private static void ValidateRequest(
        CalculateSettlementCommand command)
    {
        if (command.Request.AgencyId <= 0)
        {
            throw new ArgumentException(
                "AgencyId نامعتبر است.");
        }

        if (command.Request.YearId <= 0)
        {
            throw new ArgumentException(
                "YearId نامعتبر است.");
        }
    }

    private static SettlementResultDto CreateResult(
        Settlement settlement)
    {
        return new SettlementResultDto
        {
            SettlementId =
                settlement.Id,

            AgencyId =
                settlement.AgencyId,

            TotalDebit =
                settlement.TotalDebit,

            TotalCredit =
                settlement.TotalCredit,

            Balance =
                settlement.Balance,

            Items = settlement.Items
                .Select(x => new SettlementItemResultDto
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
                        x.CreditAmount
                })
                .ToList()
        };
    }
}



//using AgencySettlement.Application.Abstractions.External;
//using AgencySettlement.Application.Abstractions.Persistence.Repositories;
//using AgencySettlement.Application.DTOs;
//using AgencySettlement.Domain.Entities;
//using MediatR;

//namespace AgencySettlement.Application.Settlements.Commands;

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

//        /*
//         * سهمیه FreeQuotaCount بین Plan 3 و Plan 5 مشترک است.
//         *
//         * اول Plan 3
//         * بعد Plan 5
//         */
//        var groups = records
//            .GroupBy(x => new
//            {
//                x.PackageId,
//                x.EducationalLevelId,
//                x.ExamModeId,
//                x.RegistrationPlanId,
//                x.YearId
//            })
//            .OrderBy(x =>
//                GetQuotaPriority(
//                    x.Key.RegistrationPlanId));

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

//            var percent = await _percentRepository.GetAsync(
//                request.AgencyId,
//                group.Key.ExamModeId,
//                cancellationToken);

//            if (percent is null)
//                continue;

//            var candidateCount = group.Count();

//            /*
//             * ----------------------------
//             * مصرف سهمیه
//             * ----------------------------
//             */

//            var quotaCount = 0;

//            if (IsFreeQuotaPlan(
//                    group.Key.RegistrationPlanId))
//            {
//                quotaCount =
//                    await _agencyRepository.ConsumeFreeQuotaAsync(
//                        request.AgencyId,
//                        candidateCount,
//                        cancellationToken);
//            }

//            /*
//             * ----------------------------
//             * مبلغ کل گروه
//             * ----------------------------
//             *
//             * سهمیه در اینجا از BaseAmount
//             * کم نمی‌شود.
//             */
//            var baseAmount =
//                candidateCount * price.Amount;

//            /*
//             * ----------------------------
//             * محاسبه سهم‌ها
//             * ----------------------------
//             *
//             * ابتدا کل مبلغ عادی محاسبه می‌شود.
//             */
//            var agencyAmount =
//                CalculateShare(
//                    baseAmount,
//                    percent.AgencyPercent);

//            var gajAmount =
//                CalculateShare(
//                    baseAmount,
//                    percent.GajPercent);

//            var studentAmount =
//                CalculateShare(
//                    baseAmount,
//                    percent.StudentPercent);

//            /*
//             * ----------------------------
//             * Debit / Credit
//             * ----------------------------
//             */
//            var (debitAmount, creditAmount) =
//                CalculateSettlementAmount(
//                    group.Key.RegistrationPlanId,
//                    gajAmount);

//            /*
//             * ----------------------------
//             * مبلغ سهمیه
//             * ----------------------------
//             *
//             * قیمت Plan 3 و 5 در Price
//             * برابر 1,000,000 ریال است.
//             *
//             * بنابراین:
//             *
//             * 5 سهمیه × 1,000,000
//             * = 5,000,000 ریال
//             */
//            var quotaAmount =
//                quotaCount * price.Amount;

//            /*
//             * ----------------------------
//             * کسر مستقیم از Settlement
//             * ----------------------------
//             *
//             * Plan 3 و 5 بدهکار هستند.
//             *
//             * بنابراین سهمیه مستقیماً از
//             * DebitAmount کم می‌شود.
//             */
//            if (IsDebitPlan(
//                    group.Key.RegistrationPlanId))
//            {
//                debitAmount =
//                    Math.Max(
//                        0m,
//                        debitAmount - quotaAmount);
//            }
//            else if (IsCreditPlan(
//                         group.Key.RegistrationPlanId))
//            {
//                /*
//                 * فعلاً Plan 8 بدون سهمیه است،
//                 * ولی اگر در آینده سهمیه برای
//                 * Credit فعال شد، اینجا باید
//                 * منطق آن مشخص شود.
//                 */
//                creditAmount =
//                    Math.Max(
//                        0m,
//                        creditAmount - quotaAmount);
//            }

//            settlement.Items.Add(
//                new SettlementItem
//                {
//                    PackageId =
//                        group.Key.PackageId,

//                    EducationalLevelId =
//                        group.Key.EducationalLevelId,

//                    ExamModeId =
//                        group.Key.ExamModeId,

//                    RegistrationPlanId =
//                        group.Key.RegistrationPlanId,

//                    YearId =
//                        group.Key.YearId,

//                    CandidateCount =
//                        candidateCount,

//                    /*
//                     * تعداد سهمیه مصرف‌شده
//                     */
//                    FreeCandidateCount =
//                        quotaCount,

//                    /*
//                     * تعداد واقعی داوطلبان همچنان
//                     * همان تعداد اصلی است.
//                     */
//                    PaidCandidateCount =
//                        candidateCount,

//                    UnitPrice =
//                        price.Amount,

//                    /*
//                     * BaseAmount مبلغ قبل از
//                     * کسر سهمیه است.
//                     */
//                    BaseAmount =
//                        baseAmount,

//                    AgencyPercent =
//                        percent.AgencyPercent,

//                    GajPercent =
//                        percent.GajPercent,

//                    StudentPercent =
//                        percent.StudentPercent,

//                    AgencyAmount =
//                        agencyAmount,

//                    GajAmount =
//                        gajAmount,

//                    StudentAmount =
//                        studentAmount,

//                    /*
//                     * مبلغ نهایی Settlement
//                     * بعد از کسر سهمیه
//                     */
//                    DebitAmount =
//                        debitAmount,

//                    CreditAmount =
//                        creditAmount,

//                    CreatedAt =
//                        DateTime.UtcNow
//                });
//        }

//        if (settlement.Items.Count == 0)
//        {
//            throw new InvalidOperationException(
//                "هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
//        }

//        settlement.TotalDebit =
//            settlement.Items.Sum(x => x.DebitAmount);

//        settlement.TotalCredit =
//            settlement.Items.Sum(x => x.CreditAmount);

//        settlement.Balance =
//            settlement.TotalDebit -
//            settlement.TotalCredit;

//        /*
//         * Save Settlement
//         */
//        await _settlementRepository.AddAsync(
//            settlement,
//            cancellationToken);

//        await _settlementRepository.SaveChangesAsync(
//            cancellationToken);

//        /*
//         * History
//         */
//        var history = new SettlementHistory
//        {
//            SettlementId =
//                settlement.Id,

//            AgencyId =
//                settlement.AgencyId,

//            TotalDebit =
//                settlement.TotalDebit,

//            TotalCredit =
//                settlement.TotalCredit,

//            Balance =
//                settlement.Balance,

//            CreatedAt =
//                DateTime.UtcNow,

//            Description =
//                "محاسبه Settlement نماینده"
//        };

//        await _historyRepository.AddAsync(
//            history,
//            cancellationToken);

//        await _settlementRepository.SaveChangesAsync(
//            cancellationToken);

//        return CreateResult(settlement);
//    }

//    private static bool IsFreeQuotaPlan(
//        int registrationPlanId)
//    {
//        return registrationPlanId is 3 or 5;
//    }

//    private static int GetQuotaPriority(
//        int registrationPlanId)
//    {
//        return registrationPlanId switch
//        {
//            3 => 1,
//            5 => 2,
//            _ => 3
//        };
//    }

//    private static bool IsDebitPlan(
//        int registrationPlanId)
//    {
//        return registrationPlanId is 1 or 3 or 5;
//    }

//    private static bool IsCreditPlan(
//        int registrationPlanId)
//    {
//        return registrationPlanId is 2 or 8;
//    }

//    private static decimal CalculateShare(
//        decimal baseAmount,
//        decimal percent)
//    {
//        return baseAmount * percent / 100m;
//    }

//    private static (decimal Debit, decimal Credit)
//        CalculateSettlementAmount(
//            int registrationPlanId,
//            decimal gajAmount)
//    {
//        return registrationPlanId switch
//        {
//            // بدهکار
//            1 => (gajAmount, 0m),
//            3 => (gajAmount, 0m),
//            5 => (gajAmount, 0m),

//            // بستانکار
//            2 => (0m, gajAmount),
//            8 => (0m, gajAmount),

//            _ => (0m, 0m)
//        };
//    }

//    private static void ValidateRequest(
//        CalculateSettlementCommand command)
//    {
//        if (command.Request.AgencyId <= 0)
//        {
//            throw new ArgumentException(
//                "AgencyId نامعتبر است.");
//        }

//        if (command.Request.YearId <= 0)
//        {
//            throw new ArgumentException(
//                "YearId نامعتبر است.");
//        }
//    }

//    private static SettlementResultDto CreateResult(
//        Settlement settlement)
//    {
//        return new SettlementResultDto
//        {
//            SettlementId =
//                settlement.Id,

//            AgencyId =
//                settlement.AgencyId,

//            TotalDebit =
//                settlement.TotalDebit,

//            TotalCredit =
//                settlement.TotalCredit,

//            Balance =
//                settlement.Balance,

//            Items = settlement.Items
//                .Select(x => new SettlementItemResultDto
//                {
//                    PackageId =
//                        x.PackageId,

//                    EducationalLevelId =
//                        x.EducationalLevelId,

//                    StudyFieldId =
//                        x.StudyFieldId,

//                    ExamModeId =
//                        x.ExamModeId,

//                    RegistrationPlanId =
//                        x.RegistrationPlanId,

//                    YearId =
//                        x.YearId,

//                    CandidateCount =
//                        x.CandidateCount,

//                    FreeCandidateCount =
//                        x.FreeCandidateCount,

//                    PaidCandidateCount =
//                        x.PaidCandidateCount,

//                    UnitPrice =
//                        x.UnitPrice,

//                    BaseAmount =
//                        x.BaseAmount,

//                    AgencyPercent =
//                        x.AgencyPercent,

//                    GajPercent =
//                        x.GajPercent,

//                    StudentPercent =
//                        x.StudentPercent,

//                    AgencyAmount =
//                        x.AgencyAmount,

//                    GajAmount =
//                        x.GajAmount,

//                    StudentAmount =
//                        x.StudentAmount,

//                    DebitAmount =
//                        x.DebitAmount,

//                    CreditAmount =
//                        x.CreditAmount
//                })
//                .ToList()
//        };
//    }
//}
