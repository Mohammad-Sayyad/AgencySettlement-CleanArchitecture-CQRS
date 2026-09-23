
//using AgencySettlement.Application.Abstractions.External;
//using AgencySettlement.Application.Abstractions.Persistence.Repositories;
//using AgencySettlement.Application.Settlements.Commands;
//using AgencySettlement.Domain.Entities;
//using MediatR;
//using System;
//using System.Threading;
//using static System.Net.Mime.MediaTypeNames;

//namespace AgencySettlement.Application.Features.Settlements.Handlers;

//public sealed class CalculateSettlementCommandHandler
//    : IRequestHandler<CalculateSettlementCommand, SettlementResultDto>
//{
//    private const int RegularPlanId = 1;
//    private const int HekmatPlanId = 2;
//    private const int SchoolScholarshipPlanId = 3;
//    private const int FreeVolunteerPlanId = 5;
//    private const int SiteRegistrationPlanId = 8;

//    private const int OnlineExamModeId = 1;

//    private const decimal OneHundredThousandTomanInRial =
//        1_000_000m;

//    private readonly IExternalExamRecordRepository _externalRepository;
//    private readonly IPriceRepository _priceRepository;
//    private readonly IPercentRuleRepository _percentRepository;
//    private readonly IAgencyRepository _agencyRepository;
//    private readonly ISettlementRepository _settlementRepository;
//    private readonly ISettlementOrderRepository _settlementOrderRepository;
//    private readonly ISettlementHistoryRepository _historyRepository;

//    public CalculateSettlementCommandHandler(
//        IExternalExamRecordRepository externalRepository,
//        IPriceRepository priceRepository,
//        IPercentRuleRepository percentRepository,
//        IAgencyRepository agencyRepository,
//        ISettlementRepository settlementRepository,
//        ISettlementOrderRepository settlementOrderRepository,
//        ISettlementHistoryRepository historyRepository)
//    {
//        _externalRepository = externalRepository;
//        _priceRepository = priceRepository;
//        _percentRepository = percentRepository;
//        _agencyRepository = agencyRepository;
//        _settlementRepository = settlementRepository;
//        _settlementOrderRepository = settlementOrderRepository;
//        _historyRepository = historyRepository;
//    }

//    public async Task<SettlementResultDto> Handle(
//        CalculateSettlementCommand command,
//        CancellationToken cancellationToken)
//    {
//        ValidateRequest(command);

//        var request = command.Request;

//        var records =
//            await _externalRepository.GetByAgencyAndYearAsync(
//                request.AgencyId,
//                request.YearId,
//                request.PersianExecutionDate,
//                request.RegistrationOrder,
//                cancellationToken);

//        if (records.Count == 0)
//        {
//            throw new InvalidOperationException(
//                $"هیچ رکوردی برای نماینده {request.AgencyId}، سال {request.YearId}، تاریخ {request.PersianExecutionDate} و Order {request.RegistrationOrder} پیدا نشد.");
//        }

//        var orderRecords = records
//            .Where(x =>
//                x.RegistrationOrder ==
//                request.RegistrationOrder)
//            .ToList();

//        if (orderRecords.Count == 0)
//        {
//            throw new InvalidOperationException(
//                $"هیچ رکوردی برای RegistrationOrder={request.RegistrationOrder} پیدا نشد.");
//        }

//        var agency =
//            await _agencyRepository.GetByIdAsync(
//                request.AgencyId,
//                cancellationToken);

//        if (agency is null)
//        {
//            throw new InvalidOperationException(
//                "نماینده پیدا نشد.");
//        }

//        /*
//         * فقط همان Order که در Request ارسال شده
//         * محاسبه و ذخیره می‌شود.
//         */
//        var calculatedOrder =
//            await CalculateOrderAsync(
//                orderRecords,
//                request.RegistrationOrder,
//                agency,
//                cancellationToken);

//        /*
//         * Orderهای موجود را از DB می‌خوانیم تا Settlement
//         * مجموع Order 1 و Order 2 موجود را نشان دهد.
//         *
//         * نکته:
//         * اینجا هیچ Order دیگری Calculate نمی‌شود.
//         */
//        var order1 =
//            await _settlementOrderRepository
//                .GetByAgencyYearAndOrderAsync(
//                    request.AgencyId,
//                    request.YearId,
//                    1,
//                    cancellationToken);

//        var order2 =
//            await _settlementOrderRepository
//                .GetByAgencyYearAndOrderAsync(
//                    request.AgencyId,
//                    request.YearId,
//                    2,
//                    cancellationToken);

//        var settlement =
//            await _settlementRepository.GetByAgencyAndYearAsync(
//                request.AgencyId,
//                request.YearId,
//                cancellationToken);

//        if (settlement is null)
//        {
//            settlement = new Settlement
//            {
//                AgencyId = request.AgencyId,
//                YearId = request.YearId,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _settlementRepository.AddAsync(
//                settlement,
//                cancellationToken);
//        }

//        UpdateSettlement(
//            settlement,
//            order1,
//            order2);

//        await _settlementRepository.SaveChangesAsync(
//            cancellationToken);

//        var history = new SettlementHistory
//        {
//            SettlementId =
//                settlement.Id,

//            AgencyId =
//                settlement.AgencyId,

//            YearId =
//                settlement.YearId,

//            PersianExecutionDate =
//                settlement.PersianExecutionDate,

//            ContractFloorAmount =
//                settlement.ContractFloorAmount,

//            TotalDebit =
//                settlement.TotalDebit,

//            TotalCredit =
//                settlement.TotalCredit,

//            Balance =
//                settlement.Balance,

//            TotalDebitGaj =
//                settlement.TotalDebitGaj,

//            TotalCreditGaj =
//                settlement.TotalCreditGaj,

//            BalanceGaj =
//                settlement.BalanceGaj,

//            CreatedAt =
//                DateTime.UtcNow,

//            Description =
//                $"محاسبه Settlement نماینده - Order {request.RegistrationOrder}"
//        };

//        await _historyRepository.AddAsync(
//            history,
//            cancellationToken);

//        await _settlementRepository.SaveChangesAsync(
//            cancellationToken);

//        /*
//         * Response فقط Order فعلی را برمی‌گرداند.
//         *
//         * اما خود Settlement در DB شامل مجموع Orderهای
//         * موجود است.
//         */
//        return CreateResult(
//            settlement,
//            calculatedOrder);
//    }

//    private async Task<SettlementOrder> CalculateOrderAsync(
//        List<ExternalExamRecord> records,
//        int registrationOrder,
//        Agency agency,
//        CancellationToken cancellationToken)
//    {
//        var existingOrder =
//            await _settlementOrderRepository
//                .GetByAgencyYearAndOrderAsync(
//                    agency.Id,
//                    records[0].YearId,
//                    registrationOrder,
//                    cancellationToken);

//        var existingPaidAmount = 0m;

//        if (existingOrder is not null)
//        {
//            existingPaidAmount =
//                existingOrder.TotalCredit -
//                existingOrder.Items.Sum(
//                    x => x.CreditAmount);

//            if (existingPaidAmount < 0m)
//            {
//                existingPaidAmount = 0m;
//            }

//            existingOrder.Items.Clear();
//        }

//        var order =
//            existingOrder
//            ?? new SettlementOrder
//            {
//                AgencyId =
//                    agency.Id,

//                YearId =
//                    records[0].YearId,

//                RegistrationOrder =
//                    registrationOrder,

//                CreatedAt =
//                    DateTime.UtcNow
//            };

//        decimal totalDebit = 0m;
//        decimal totalCredit = 0m;
//        decimal totalCreditGaj = 0m;

//        var groups = records
//            .GroupBy(x =>
//                new SettlementGroupKey(
//                    x.PackageId,
//                    x.EducationalLevelId,
//                    x.ExamModeId,
//                    x.RegistrationPlanId,
//                    x.StudyFieldId,
//                    x.YearId,
//                    x.AgencyId,
//                    x.PersianExecutionDate))
//            .ToList();

//        /*
//         * سهمیه Plan 5 فقط برای همین محاسبه Order مصرف می‌شود.
//         *
//         * Plan 3 اصلاً از سهمیه استفاده نمی‌کند.
//         */
//        var plan5QuotaState =
//            new Plan5QuotaState(
//                agency.FreeQuotaCount,
//                agency.OneHundredThousandQuotaCount);

//        foreach (var group in groups)
//        {
//            var planId =
//                group.Key.RegistrationPlanId;

//            var candidateCount =
//                group.Count();

//            var unitPrice =
//                await GetUnitPriceAsync(
//                    group.Key.PackageId,
//                    group.Key.EducationalLevelId,
//                    group.Key.ExamModeId,
//                    planId,
//                    group.Key.YearId,
//                    cancellationToken);

//            if (unitPrice is null)
//            {
//                continue;
//            }

//            var calculation =
//                planId switch
//                {
//                    SchoolScholarshipPlanId =>
//                        CalculateSchoolScholarship(
//                            group.Key.ExamModeId,
//                            candidateCount,
//                            unitPrice.Value),

//                    FreeVolunteerPlanId =>
//                        CalculateFreeVolunteer(
//                            group.Key.ExamModeId,
//                            candidateCount,
//                            unitPrice.Value,
//                            plan5QuotaState),

//                    HekmatPlanId =>
//                        CalculateHekmat(
//                            candidateCount,
//                            unitPrice.Value),

//                    RegularPlanId or SiteRegistrationPlanId =>
//                        await CalculateRegularOrSiteAsync(
//                            planId,
//                            group.Key.ExamModeId,
//                            candidateCount,
//                            unitPrice.Value,
//                            group.Key.AgencyId,
//                            cancellationToken),

//                    _ =>
//                        GroupCalculation.Invalid()
//                };

//            if (!calculation.IsValid)
//            {
//                continue;
//            }

//            totalDebit +=
//                calculation.DebitAmount;

//            totalCredit +=
//                calculation.CreditAmount;

//            totalCreditGaj +=
//                calculation.TotalCreditGaj;

//            order.Items.Add(
//                CreateSettlementOrderItem(
//                    group.Key,
//                    candidateCount,
//                    calculation,
//                    agency));
//        }

//        if (order.Items.Count == 0)
//        {
//            throw new InvalidOperationException(
//                $"برای Order {registrationOrder} هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
//        }

//        order.PersianExecutionDate =
//            records[0].PersianExecutionDate;

//        order.ContractFloorAmount =
//            agency.ContractFloorAmount;

//        order.TotalDebit =
//            Math.Max(
//                0m,
//                totalDebit);

//        /*
//         * مبلغ پرداختی قبلی که خارج از Itemهای فعلی بوده
//         * حفظ می‌شود.
//         */
//        order.TotalCredit =
//            Math.Max(
//                0m,
//                totalCredit +
//                existingPaidAmount);

//        order.Balance =
//            order.TotalDebit -
//            order.TotalCredit;

//        /*
//         * از دید Gaj:
//         * TotalDebitGaj = مبلغی که Gaj به Agency بدهکار است
//         * TotalCreditGaj = مبلغی که Agency به Gaj بدهکار است
//         */
//        order.TotalDebitGaj =
//            totalCredit;

//        order.TotalCreditGaj =
//            totalCreditGaj;

//        order.BalanceGaj =
//            Math.Max(
//                0m,
//                order.TotalCreditGaj -
//                order.TotalDebit);

//        if (existingOrder is null)
//        {
//            await _settlementOrderRepository.AddAsync(
//                order,
//                cancellationToken);
//        }

//        await _settlementOrderRepository.SaveChangesAsync(
//            cancellationToken);

//        return order;
//    }

//    private static void UpdateSettlement(
//        Settlement settlement,
//        SettlementOrder? order1,
//        SettlementOrder? order2)
//    {
//        settlement.TotalDebit =
//            (order1?.TotalDebit ?? 0m) +
//            (order2?.TotalDebit ?? 0m);

//        settlement.TotalCredit =
//            (order1?.TotalCredit ?? 0m) +
//            (order2?.TotalCredit ?? 0m);

//        settlement.Balance =
//            settlement.TotalDebit -
//            settlement.TotalCredit;

//        settlement.TotalDebitGaj =
//            (order1?.TotalDebitGaj ?? 0m) +
//            (order2?.TotalDebitGaj ?? 0m);

//        settlement.TotalCreditGaj =
//            (order1?.TotalCreditGaj ?? 0m) +
//            (order2?.TotalCreditGaj ?? 0m);

//        settlement.BalanceGaj =
//            Math.Max(
//                0m,
//                settlement.TotalCreditGaj -
//                settlement.TotalDebit);

//        /*
//         * Order فعلی یا آخرین Order موجود
//         * تاریخ و کف قرارداد Settlement را تعیین می‌کند.
//         */
//        var latestOrder =
//            order2 ??
//            order1;

//        if (latestOrder is not null)
//        {
//            settlement.ContractFloorAmount =
//                latestOrder.ContractFloorAmount;

//            settlement.PersianExecutionDate =
//                latestOrder.PersianExecutionDate;
//        }
//    }

//    private static GroupCalculation CalculateSchoolScholarship(
//        int examModeId,
//        int candidateCount,
//        decimal unitPrice)
//    {
//        /*
//         * Plan 3 آنلاین برای همه رایگان است.
//         */
//        if (examModeId == OnlineExamModeId)
//        {
//            return GroupCalculation.Free(
//                candidateCount,
//                unitPrice);
//        }

//        /*
//         * Plan 3 حضوری همیشه 100,000 تومان است
//         * و اصلاً از سهمیه استفاده نمی‌کند.
//         */
//        var baseAmount =
//            candidateCount *
//            OneHundredThousandTomanInRial;

//        return GroupCalculation.Debit(
//            candidateCount,
//            freeCandidateCount: 0,
//            paidCandidateCount: candidateCount,
//            unitPrice,
//            baseAmount);
//    }

//    private static GroupCalculation CalculateFreeVolunteer(
//        int examModeId,
//        int candidateCount,
//        decimal normalUnitPrice,
//        Plan5QuotaState quotaState)
//    {
//        /*
//         * Plan 5 آنلاین برای همه رایگان است.
//         */
//        if (examModeId == OnlineExamModeId)
//        {
//            return GroupCalculation.Free(
//                candidateCount,
//                normalUnitPrice);
//        }

//        /*
//         * Plan 5 حضوری:
//         *
//         * 1. سهمیه رایگان
//         * 2. سهمیه 100 هزار تومان
//         * 3. مابقی با قیمت عادی
//         */
//        var freeCandidateCount =
//            Math.Min(
//                quotaState.RemainingFreeQuota,
//                candidateCount);

//        quotaState.RemainingFreeQuota -=
//            freeCandidateCount;

//        var remainingCandidateCount =
//            candidateCount -
//            freeCandidateCount;

//        var oneHundredThousandCandidateCount =
//            Math.Min(
//                quotaState.RemainingOneHundredThousandQuota,
//                remainingCandidateCount);

//        quotaState.RemainingOneHundredThousandQuota -=
//            oneHundredThousandCandidateCount;

//        var normalCandidateCount =
//            remainingCandidateCount -
//            oneHundredThousandCandidateCount;

//        var baseAmount =
//            oneHundredThousandCandidateCount *
//            OneHundredThousandTomanInRial;

//        baseAmount +=
//            normalCandidateCount *
//            normalUnitPrice;

//        var paidCandidateCount =
//            candidateCount -
//            freeCandidateCount;

//        return GroupCalculation.Debit(
//            candidateCount,
//            freeCandidateCount,
//            paidCandidateCount,
//            normalUnitPrice,
//            baseAmount);
//    }

//    private static GroupCalculation CalculateHekmat(
//        int candidateCount,
//        decimal unitPrice)
//    {
//        var baseAmount =
//            candidateCount *
//            unitPrice;

//        return GroupCalculation.Credit(
//            candidateCount,
//            unitPrice,
//            baseAmount);
//    }

//    private async Task<GroupCalculation> CalculateRegularOrSiteAsync(
//        int registrationPlanId,
//        int examModeId,
//        int candidateCount,
//        decimal unitPrice,
//        int requestAgencyId,
//        CancellationToken cancellationToken)
//    {
//        var percent =
//            await _percentRepository.GetAsync(
//                requestAgencyId,
//                examModeId,
//                cancellationToken);

//        if (percent is null)
//        {
//            return GroupCalculation.Invalid();
//        }

//        var baseAmount =
//            candidateCount *
//            unitPrice;

//        var agencyAmount =
//            CalculateShare(
//                baseAmount,
//                registrationPlanId ==
//                SiteRegistrationPlanId
//                    ? percent.AgencyPercent
//                    : percent.GajPercent);

//        var gajAmount =
//            CalculateShare(
//                baseAmount,
//                registrationPlanId ==
//                SiteRegistrationPlanId
//                    ? percent.GajPercent
//                    : percent.AgencyPercent);

//        var studentAmount =
//            CalculateShare(
//                baseAmount,
//                percent.StudentPercent);

//        if (registrationPlanId == RegularPlanId)
//        {
//            return GroupCalculation.Regular(
//                candidateCount,
//                unitPrice,
//                baseAmount,
//                percent.AgencyPercent,
//                percent.GajPercent,
//                percent.StudentPercent,
//                agencyAmount,
//                gajAmount,
//                studentAmount);
//        }

//        if (registrationPlanId == SiteRegistrationPlanId)
//        {
//            return GroupCalculation.Site(
//                candidateCount,
//                unitPrice,
//                baseAmount,
//                percent.AgencyPercent,
//                percent.GajPercent,
//                percent.StudentPercent,
//                agencyAmount,
//                gajAmount,
//                studentAmount);
//        }

//        return GroupCalculation.Invalid();
//    }

//    private async Task<decimal?> GetUnitPriceAsync(
//        int packageId,
//        int educationalLevelId,
//        int examModeId,
//        int registrationPlanId,
//        int yearId,
//        CancellationToken cancellationToken)
//    {
//        var price =
//            await _priceRepository.GetAsync(
//                packageId,
//                educationalLevelId,
//                examModeId,
//                registrationPlanId,
//                yearId,
//                cancellationToken);

//        return price?.Amount;
//    }

//    private static SettlementOrderItem CreateSettlementOrderItem(
//        SettlementGroupKey key,
//        int candidateCount,
//        GroupCalculation calculation,
//        Agency agency)
//    {
//        return new SettlementOrderItem
//        {
//            SettlementOrderId = 0,

//            PackageId =
//                key.PackageId,

//            EducationalLevelId =
//                key.EducationalLevelId,

//            ExamModeId =
//                key.ExamModeId,

//            RegistrationPlanId =
//                key.RegistrationPlanId,

//            YearId =
//                key.YearId,

//            StudyFieldId =
//                key.StudyFieldId,

//            PersianExecutionDate =
//                key.PersianExecutionDate,

//            AgencyId =
//                key.AgencyId,

//            CandidateCount =
//                candidateCount,

//            FreeCandidateCount =
//                calculation.FreeCandidateCount,

//            PaidCandidateCount =
//                calculation.PaidCandidateCount,

//            FreeQuotaCount =
//                key.RegistrationPlanId ==
//                FreeVolunteerPlanId
//                    ? agency.FreeQuotaCount
//                    : 0,

//            OneHundredThousandQuotaCount =
//                key.RegistrationPlanId ==
//                FreeVolunteerPlanId
//                    ? agency.OneHundredThousandQuotaCount
//                    : 0,

//            ContractFloorAmount =
//                agency.ContractFloorAmount,

//            UnitPrice =
//                calculation.UnitPrice,

//            BaseAmount =
//                calculation.BaseAmount,

//            AgencyPercent =
//                calculation.AgencyPercent,

//            GajPercent =
//                calculation.GajPercent,

//            StudentPercent =
//                calculation.StudentPercent,

//            AgencyAmount =
//                calculation.AgencyAmount,

//            GajAmount =
//                calculation.GajAmount,

//            StudentAmount =
//                calculation.StudentAmount,

//            DebitAmount =
//                calculation.DebitAmount,

//            CreditAmount =
//                calculation.CreditAmount,

//            CreatedAt =
//                DateTime.UtcNow
//        };
//    }

//    private static decimal CalculateShare(
//        decimal baseAmount,
//        decimal percent)
//    {
//        return baseAmount *
//               percent /
//               100m;
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

//        if (string.IsNullOrWhiteSpace(
//            command.Request.PersianExecutionDate))
//        {
//            throw new ArgumentException(
//                "PersianExecutionDate الزامی است.");
//        }

//        if (command.Request.RegistrationOrder is not (1 or 2))
//        {
//            throw new ArgumentException(
//                "RegistrationOrder باید 1 یا 2 باشد.");
//        }
//    }

//    private static SettlementResultDto CreateResult(
//        Settlement settlement,
//        SettlementOrder calculatedOrder)
//    {
//        var items =
//            calculatedOrder.Items
//                .Select(
//                    x => new SettlementItemResultDto
//                    {
//                        PackageId =
//                            x.PackageId,

//                        EducationalLevelId =
//                            x.EducationalLevelId,

//                        StudyFieldId =
//                            x.StudyFieldId,

//                        ExamModeId =
//                            x.ExamModeId,

//                        RegistrationPlanId =
//                            x.RegistrationPlanId,

//                        YearId =
//                            x.YearId,

//                        CandidateCount =
//                            x.CandidateCount,

//                        FreeCandidateCount =
//                            x.FreeCandidateCount,

//                        PaidCandidateCount =
//                            x.PaidCandidateCount,

//                        FreeQuotaCount =
//                            x.FreeQuotaCount,

//                        OneHundredThousandQuotaCount =
//                            x.OneHundredThousandQuotaCount,

//                        ContractFloorAmount =
//                            x.ContractFloorAmount,

//                        UnitPrice =
//                            x.UnitPrice,

//                        BaseAmount =
//                            x.BaseAmount,

//                        AgencyPercent =
//                            x.AgencyPercent,

//                        GajPercent =
//                            x.GajPercent,

//                        StudentPercent =
//                            x.StudentPercent,

//                        AgencyAmount =
//                            x.AgencyAmount,

//                        GajAmount =
//                            x.GajAmount,

//                        StudentAmount =
//                            x.StudentAmount,

//                        DebitAmount =
//                            x.DebitAmount,

//                        CreditAmount =
//                            x.CreditAmount,

//                        PersianExecutionDate =
//                            x.PersianExecutionDate
//                    })
//                .ToList();

//        return new SettlementResultDto
//        {
//            SettlementId =
//                settlement.Id,

//            AgencyId =
//                settlement.AgencyId,

//            ContractFloorAmount =
//                settlement.ContractFloorAmount,

//            TotalDebit =
//                settlement.TotalDebit,

//            TotalCredit =
//                settlement.TotalCredit,

//            Balance =
//                settlement.Balance,

//            TotalDebitGaj =
//                settlement.TotalDebitGaj,

//            TotalCreditGaj =
//                settlement.TotalCreditGaj,

//            BalanceGaj =
//                settlement.BalanceGaj,

//            PersianExecutionDate =
//                settlement.PersianExecutionDate,

//            Items =
//                items
//        };
//    }

//    private sealed record SettlementGroupKey(
//        int PackageId,
//        int EducationalLevelId,
//        int ExamModeId,
//        int RegistrationPlanId,
//        int StudyFieldId,
//        int YearId,
//        int AgencyId,
//        string PersianExecutionDate);

//    private sealed class Plan5QuotaState
//    {
//        public Plan5QuotaState(
//            int freeQuotaCount,
//            int oneHundredThousandQuotaCount)
//        {
//            RemainingFreeQuota =
//                Math.Max(
//                    0,
//                    freeQuotaCount);

//            RemainingOneHundredThousandQuota =
//                Math.Max(
//                    0,
//                    oneHundredThousandQuotaCount);
//        }

//        public int RemainingFreeQuota { get; set; }

//        public int RemainingOneHundredThousandQuota { get; set; }
//    }

//    private sealed record GroupCalculation(
//        bool IsValid,
//        int FreeCandidateCount,
//        int PaidCandidateCount,
//        decimal UnitPrice,
//        decimal BaseAmount,
//        decimal AgencyPercent,
//        decimal GajPercent,
//        decimal StudentPercent,
//        decimal AgencyAmount,
//        decimal GajAmount,
//        decimal StudentAmount,
//        decimal DebitAmount,
//        decimal CreditAmount,
//        decimal TotalCreditGaj)
//    {
//        public static GroupCalculation Invalid()
//        {
//            return new GroupCalculation(
//                false,
//                0,
//                0,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m);
//        }

//        public static GroupCalculation Free(
//            int candidateCount,
//            decimal unitPrice)
//        {
//            return new GroupCalculation(
//                true,
//                candidateCount,
//                0,
//                unitPrice,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m,
//                0m);
//        }

//        public static GroupCalculation Debit(
//            int candidateCount,
//            int freeCandidateCount,
//            int paidCandidateCount,
//            decimal unitPrice,
//            decimal baseAmount)
//        {
//            return new GroupCalculation(
//                true,
//                freeCandidateCount,
//                paidCandidateCount,
//                unitPrice,
//                baseAmount,
//                0m,
//                0m,
//                0m,
//                baseAmount,
//                baseAmount,
//                0m,
//                baseAmount,
//                0m,
//                0m);
//        }

//        public static GroupCalculation Credit(
//            int candidateCount,
//            decimal unitPrice,
//            decimal baseAmount)
//        {
//            return new GroupCalculation(
//                true,
//                0,
//                candidateCount,
//                unitPrice,
//                baseAmount,
//                0m,
//                0m,
//                0m,
//                baseAmount,
//                baseAmount,
//                0m,
//                0m,
//                baseAmount,
//                0m);
//        }

//        public static GroupCalculation Regular(
//            int candidateCount,
//            decimal unitPrice,
//            decimal baseAmount,
//            decimal agencyPercent,
//            decimal gajPercent,
//            decimal studentPercent,
//            decimal agencyAmount,
//            decimal gajAmount,
//            decimal studentAmount)
//        {
//            return new GroupCalculation(
//                true,
//                0,
//                candidateCount,
//                unitPrice,
//                baseAmount,
//                agencyPercent,
//                gajPercent,
//                studentPercent,
//                gajAmount,
//                agencyAmount,
//                studentAmount,
//                agencyAmount,
//                0m,
//                gajAmount);
//        }

//        public static GroupCalculation Site(
//            int candidateCount,
//            decimal unitPrice,
//            decimal baseAmount,
//            decimal agencyPercent,
//            decimal gajPercent,
//            decimal studentPercent,
//            decimal agencyAmount,
//            decimal gajAmount,
//            decimal studentAmount)
//        {
//            return new GroupCalculation(
//                true,
//                0,
//                candidateCount,
//                unitPrice,
//                baseAmount,
//                agencyPercent,
//                gajPercent,
//                studentPercent,
//                gajAmount,
//                agencyAmount,
//                studentAmount,
//                0m,
//                agencyAmount,
//                0m);
//        }
//    }
//}















////using AgencySettlement.Application.Abstractions.External;
////using AgencySettlement.Application.Abstractions.Persistence.Repositories;
////using AgencySettlement.Application.Settlements.Commands;
////using AgencySettlement.Domain.Entities;
////using MediatR;
////using System.Numerics;
////using static System.Net.Mime.MediaTypeNames;

////namespace AgencySettlement.Application.Features.Settlements.Handlers;


////public sealed class CalculateSettlementCommandHandler
////    : IRequestHandler<CalculateSettlementCommand, SettlementResultDto>
////{
////    private const int RegularPlanId = 1;
////    private const int HekmatPlanId = 2;
////    private const int SchoolScholarshipPlanId = 3;
////    private const int FreeVolunteerPlanId = 5;
////    private const int SiteRegistrationPlanId = 8;

////    private const int OnlineExamModeId = 1;

////    private const decimal OneHundredThousandTomanInRial = 1_000_000m;

////    private readonly IExternalExamRecordRepository _externalRepository;
////    private readonly IPriceRepository _priceRepository;
////    private readonly IPercentRuleRepository _percentRepository;
////    private readonly IAgencyRepository _agencyRepository;
////    private readonly ISettlementRepository _settlementRepository;
////    private readonly ISettlementHistoryRepository _historyRepository;

////    public CalculateSettlementCommandHandler(
////        IExternalExamRecordRepository externalRepository,
////        IPriceRepository priceRepository,
////        IPercentRuleRepository percentRepository,
////        IAgencyRepository agencyRepository,
////        ISettlementRepository settlementRepository,
////        ISettlementHistoryRepository historyRepository)
////    {
////        _externalRepository = externalRepository;
////        _priceRepository = priceRepository;
////        _percentRepository = percentRepository;
////        _agencyRepository = agencyRepository;
////        _settlementRepository = settlementRepository;
////        _historyRepository = historyRepository;
////    }

////    public async Task<SettlementResultDto> Handle(
////        CalculateSettlementCommand command,
////        CancellationToken cancellationToken)
////    {
////        ValidateRequest(command);

////        var request = command.Request;

////        var records = await _externalRepository.GetByAgencyAndYearAsync(
////            request.AgencyId,
////            request.YearId,
////            request.PersianExecutionDate,
////            cancellationToken);

////        if (records.Count == 0)
////        {
////            throw new InvalidOperationException(
////                "هیچ رکوردی برای این نماینده، سال و تاریخ پیدا نشد.");
////        }

////        var agency = await _agencyRepository.GetByIdAsync(
////            request.AgencyId,
////            cancellationToken);

////        if (agency is null)
////        {
////            throw new InvalidOperationException(
////                "نماینده پیدا نشد.");
////        }

////        var settlement = new Settlement
////        {
////            AgencyId = request.AgencyId,
////            YearId = request.YearId,
////            PersianExecutionDate = request.PersianExecutionDate,
////            ContractFloorAmount = agency.ContractFloorAmount,
////            CreatedAt = DateTime.UtcNow
////        };

////        decimal totalDebit = 0m;
////        decimal totalCredit = 0m;
////        decimal totalCreditGaj = 0m;

////        var groups = records
////            .GroupBy(x => new SettlementGroupKey(
////                x.PackageId,
////                x.EducationalLevelId,
////                x.ExamModeId,
////                x.RegistrationPlanId,
////                x.StudyFieldId,
////                x.YearId,
////                x.AgencyId,
////                x.PersianExecutionDate))
////            .ToList();

////        var plan5QuotaState = new Plan5QuotaState(
////            agency.FreeQuotaCount,
////            agency.OneHundredThousandQuotaCount);

////        foreach (var group in groups)
////        {
////            var planId = group.Key.RegistrationPlanId;
////            var candidateCount = group.Count();

////            var unitPrice = await GetUnitPriceAsync(
////                group.Key.PackageId,
////                group.Key.EducationalLevelId,
////                group.Key.ExamModeId,
////                planId,
////                group.Key.YearId,
////                cancellationToken);

////            if (unitPrice is null)
////            {
////                continue;
////            }

////            var calculation = planId switch
////            {
////                SchoolScholarshipPlanId =>
////                    CalculateSchoolScholarship(
////                        group.Key.ExamModeId,
////                        candidateCount,
////                        unitPrice.Value),

////                FreeVolunteerPlanId =>
////                    CalculateFreeVolunteer(
////                        group.Key.ExamModeId,
////                        candidateCount,
////                        unitPrice.Value,
////                        plan5QuotaState),

////                HekmatPlanId =>
////                    CalculateHekmat(
////                        candidateCount,
////                        unitPrice.Value),

////                RegularPlanId or SiteRegistrationPlanId =>
////                    await CalculateRegularOrSiteAsync(
////                        planId,
////                        group.Key.ExamModeId,
////                        candidateCount,
////                        unitPrice.Value,
////                        request.AgencyId,
////                        cancellationToken),

////                _ => GroupCalculation.Invalid()
////            };

////            if (!calculation.IsValid)
////            {
////                continue;
////            }

////            totalDebit += calculation.DebitAmount;
////            totalCredit += calculation.CreditAmount;
////            totalCreditGaj += calculation.TotalCreditGaj;

////            settlement.Items.Add(
////                CreateSettlementItem(
////                    group.Key,
////                    candidateCount,
////                    calculation,
////                    agency));
////        }

////        if (settlement.Items.Count == 0)
////        {
////            throw new InvalidOperationException(
////                "هیچ ترکیبی دارای قیمت و درصد معتبر برای محاسبه نبود.");
////        }

////        settlement.TotalDebit =
////            Math.Max(0m, totalDebit);

////        settlement.TotalCredit =
////            Math.Max(0m, totalCredit);

////        settlement.Balance =
////            settlement.TotalDebit -
////            settlement.TotalCredit;

////        settlement.TotalDebitGaj =
////            settlement.TotalCredit;

////        settlement.TotalCreditGaj =
////            totalCreditGaj;

////        settlement.BalanceGaj =
////            Math.Max(
////                0m,
////                settlement.TotalCreditGaj -
////                settlement.TotalDebit);

////        await _settlementRepository.AddAsync(
////            settlement,
////            cancellationToken);

////        await _settlementRepository.SaveChangesAsync(
////            cancellationToken);

////        var history = new SettlementHistory
////        {
////            SettlementId = settlement.Id,
////            AgencyId = settlement.AgencyId,
////            YearId = settlement.YearId,
////            PersianExecutionDate = settlement.PersianExecutionDate,
////            ContractFloorAmount = settlement.ContractFloorAmount,
////            TotalDebit = settlement.TotalDebit,
////            TotalCredit = settlement.TotalCredit,
////            Balance = settlement.Balance,
////            TotalDebitGaj = settlement.TotalDebitGaj,
////            TotalCreditGaj = settlement.TotalCreditGaj,
////            BalanceGaj = settlement.BalanceGaj,
////            CreatedAt = DateTime.UtcNow,
////            Description = "محاسبه Settlement نماینده"
////        };

////        await _historyRepository.AddAsync(
////            history,
////            cancellationToken);

////        await _settlementRepository.SaveChangesAsync(
////            cancellationToken);

////        return CreateResult(settlement);
////    }

////    private static GroupCalculation CalculateSchoolScholarship(
////        int examModeId,
////        int candidateCount,
////        decimal unitPrice)
////    {
////        if (examModeId == OnlineExamModeId)
////        {
////            return GroupCalculation.Free(
////                candidateCount,
////                unitPrice);
////        }

////        var baseAmount =
////            candidateCount *
////            OneHundredThousandTomanInRial;

////        return GroupCalculation.Debit(
////            candidateCount,
////            freeCandidateCount: 0,
////            paidCandidateCount: candidateCount,
////            unitPrice,
////            baseAmount);
////    }

////    private static GroupCalculation CalculateFreeVolunteer(
////        int examModeId,
////        int candidateCount,
////        decimal normalUnitPrice,
////        Plan5QuotaState quotaState)
////    {
////        if (examModeId == OnlineExamModeId)
////        {
////            return GroupCalculation.Free(
////                candidateCount,
////                normalUnitPrice);
////        }

////        var freeCandidateCount =
////            Math.Min(
////                quotaState.RemainingFreeQuota,
////                candidateCount);

////        quotaState.RemainingFreeQuota -=
////            freeCandidateCount;

////        var remainingCandidateCount =
////            candidateCount -
////            freeCandidateCount;

////        var oneHundredThousandCandidateCount =
////            Math.Min(
////                quotaState.RemainingOneHundredThousandQuota,
////                remainingCandidateCount);

////        quotaState.RemainingOneHundredThousandQuota -=
////            oneHundredThousandCandidateCount;

////        var normalCandidateCount =
////            remainingCandidateCount -
////            oneHundredThousandCandidateCount;

////        var baseAmount =
////            oneHundredThousandCandidateCount *
////            OneHundredThousandTomanInRial;

////        baseAmount +=
////            normalCandidateCount *
////            normalUnitPrice;

////        var paidCandidateCount =
////            candidateCount -
////            freeCandidateCount;

////        return GroupCalculation.Debit(
////            candidateCount,
////            freeCandidateCount,
////            paidCandidateCount,
////            normalUnitPrice,
////            baseAmount);
////    }

////    private static GroupCalculation CalculateHekmat(
////        int candidateCount,
////        decimal unitPrice)
////    {
////        var baseAmount =
////            candidateCount *
////            unitPrice;

////        return GroupCalculation.Credit(
////            candidateCount,
////            unitPrice,
////            baseAmount);
////    }

////    //private async Task<GroupCalculation> CalculateRegularOrSiteAsync(
////    //    int registrationPlanId,
////    //    int examModeId,
////    //    int candidateCount,
////    //    decimal unitPrice,
////    //    int agencyId,
////    //    CancellationToken cancellationToken)
////    //{
////    //    var percent =
////    //        await _percentRepository.GetAsync(
////    //            agencyId,
////    //            examModeId,
////    //            cancellationToken);

////    //    if (percent is null)
////    //    {
////    //        return GroupCalculation.Invalid();
////    //    }

////    //    var baseAmount =
////    //        candidateCount *
////    //        unitPrice;

////    //    var gajAmount =
////    //        CalculateShare(
////    //            baseAmount,
////    //            percent.AgencyPercent);

////    //    var agencyAmount =
////    //        CalculateShare(
////    //            baseAmount,
////    //            percent.GajPercent);

////    //    var studentAmount =
////    //        CalculateShare(
////    //            baseAmount,
////    //            percent.StudentPercent);

////    //    if (registrationPlanId == RegularPlanId)
////    //    {
////    //        return GroupCalculation.Regular(
////    //            candidateCount,
////    //            unitPrice,
////    //            baseAmount,
////    //            percent.AgencyPercent,
////    //            percent.GajPercent,
////    //            percent.StudentPercent,
////    //            agencyAmount,
////    //            gajAmount,
////    //            studentAmount);
////    //    }

////    //    if (registrationPlanId == SiteRegistrationPlanId)
////    //    {
////    //        return GroupCalculation.Site(
////    //            candidateCount,
////    //            unitPrice,
////    //            baseAmount,
////    //            percent.AgencyPercent,
////    //            percent.GajPercent,
////    //            percent.StudentPercent,
////    //            agencyAmount,
////    //            gajAmount,
////    //            studentAmount);
////    //    }

////    //    return GroupCalculation.Invalid();
////    //}


////private async Task<GroupCalculation> CalculateRegularOrSiteAsync(
////    int registrationPlanId,
////    int examModeId,
////    int candidateCount,
////    decimal unitPrice,
////    int agencyId,
////    CancellationToken cancellationToken)
////    {
////        var percent =
////            await _percentRepository.GetAsync(
////                agencyId,
////                examModeId,
////                cancellationToken);

////        if (percent is null)
////        {
////            return GroupCalculation.Invalid();
////        }

////        var baseAmount =
////            candidateCount *
////            unitPrice;

////        var agencyAmount =
////            CalculateShare(
////                baseAmount,
////                registrationPlanId == SiteRegistrationPlanId
////                    ? percent.AgencyPercent
////                    : percent.GajPercent);

////        var gajAmount =
////            CalculateShare(
////                baseAmount,
////                registrationPlanId == SiteRegistrationPlanId
////                    ? percent.GajPercent
////                    : percent.AgencyPercent);

////        var studentAmount =
////            CalculateShare(
////                baseAmount,
////                percent.StudentPercent);

////        if (registrationPlanId == RegularPlanId)
////        {
////            return GroupCalculation.Regular(
////                candidateCount,
////                unitPrice,
////                baseAmount,
////                percent.AgencyPercent,
////                percent.GajPercent,
////                percent.StudentPercent,
////                agencyAmount,
////                gajAmount,
////                studentAmount);
////        }

////        if (registrationPlanId == SiteRegistrationPlanId)
////        {
////            return GroupCalculation.Site(
////                candidateCount,
////                unitPrice,
////                baseAmount,
////                percent.AgencyPercent,
////                percent.GajPercent,
////                percent.StudentPercent,
////                agencyAmount,
////                gajAmount,
////                studentAmount);
////        }

////        return GroupCalculation.Invalid();
////    }



////    private async Task<decimal?> GetUnitPriceAsync(
////        int packageId,
////        int educationalLevelId,
////        int examModeId,
////        int registrationPlanId,
////        int yearId,
////        CancellationToken cancellationToken)
////    {
////        var price = await _priceRepository.GetAsync(
////            packageId,
////            educationalLevelId,
////            examModeId,
////            registrationPlanId,
////            yearId,
////            cancellationToken);

////        return price?.Amount;
////    }

////    private static SettlementItem CreateSettlementItem(
////        SettlementGroupKey key,
////        int candidateCount,
////        GroupCalculation calculation,
////        Agency agency)
////    {
////        return new SettlementItem
////        {
////            PackageId =
////                key.PackageId,

////            EducationalLevelId =
////                key.EducationalLevelId,

////            ExamModeId =
////                key.ExamModeId,

////            RegistrationPlanId =
////                key.RegistrationPlanId,

////            YearId =
////                key.YearId,

////            StudyFieldId =
////                key.StudyFieldId,

////            PersianExecutionDate =
////                key.PersianExecutionDate,

////            AgencyId =
////                key.AgencyId,

////            CandidateCount =
////                candidateCount,

////            FreeCandidateCount =
////                calculation.FreeCandidateCount,

////            PaidCandidateCount =
////                calculation.PaidCandidateCount,

////            FreeQuotaCount =
////                key.RegistrationPlanId == FreeVolunteerPlanId
////                    ? agency.FreeQuotaCount
////                    : 0,

////            OneHundredThousandQuotaCount =
////                key.RegistrationPlanId == FreeVolunteerPlanId
////                    ? agency.OneHundredThousandQuotaCount
////                    : 0,

////            ContractFloorAmount =
////                agency.ContractFloorAmount,

////            UnitPrice =
////                calculation.UnitPrice,

////            BaseAmount =
////                calculation.BaseAmount,

////            AgencyPercent =
////                calculation.AgencyPercent,

////            GajPercent =
////                calculation.GajPercent,

////            StudentPercent =
////                calculation.StudentPercent,

////            AgencyAmount =
////                calculation.AgencyAmount,

////            GajAmount =
////                calculation.GajAmount,

////            StudentAmount =
////                calculation.StudentAmount,

////            DebitAmount =
////                calculation.DebitAmount,

////            CreditAmount =
////                calculation.CreditAmount,

////            CreatedAt =
////                DateTime.UtcNow
////        };
////    }

////    private static decimal CalculateShare(
////        decimal baseAmount,
////        decimal percent)
////    {
////        return baseAmount *
////               percent /
////               100m;
////    }

////    private static void ValidateRequest(
////        CalculateSettlementCommand command)
////    {
////        if (command.Request.AgencyId <= 0)
////        {
////            throw new ArgumentException(
////                "AgencyId نامعتبر است.");
////        }

////        if (command.Request.YearId <= 0)
////        {
////            throw new ArgumentException(
////                "YearId نامعتبر است.");
////        }

////        if (string.IsNullOrWhiteSpace(
////            command.Request.PersianExecutionDate))
////        {
////            throw new ArgumentException(
////                "PersianExecutionDate الزامی است.");
////        }
////    }

////    private static SettlementResultDto CreateResult(
////        Settlement settlement)
////    {
////        return new SettlementResultDto
////        {
////            SettlementId =
////                settlement.Id,

////            AgencyId =
////                settlement.AgencyId,

////            ContractFloorAmount =
////                settlement.ContractFloorAmount,

////            TotalDebit =
////                settlement.TotalDebit,

////            TotalCredit =
////                settlement.TotalCredit,

////            Balance =
////                settlement.Balance,

////            TotalDebitGaj =
////                settlement.TotalDebitGaj,

////            TotalCreditGaj =
////                settlement.TotalCreditGaj,

////            BalanceGaj =
////                settlement.BalanceGaj,

////            PersianExecutionDate =
////                settlement.PersianExecutionDate,

////            Items =
////                settlement.Items
////                    .Select(
////                        x => new SettlementItemResultDto
////                        {
////                            PackageId =
////                                x.PackageId,

////                            EducationalLevelId =
////                                x.EducationalLevelId,

////                            StudyFieldId =
////                                x.StudyFieldId,

////                            ExamModeId =
////                                x.ExamModeId,

////                            RegistrationPlanId =
////                                x.RegistrationPlanId,

////                            YearId =
////                                x.YearId,

////                            CandidateCount =
////                                x.CandidateCount,

////                            FreeCandidateCount =
////                                x.FreeCandidateCount,

////                            PaidCandidateCount =
////                                x.PaidCandidateCount,

////                            FreeQuotaCount =
////                                x.FreeQuotaCount,

////                            OneHundredThousandQuotaCount =
////                                x.OneHundredThousandQuotaCount,

////                            ContractFloorAmount =
////                                x.ContractFloorAmount,

////                            UnitPrice =
////                                x.UnitPrice,

////                            BaseAmount =
////                                x.BaseAmount,

////                            AgencyPercent =
////                                x.AgencyPercent,

////                            GajPercent =
////                                x.GajPercent,

////                            StudentPercent =
////                                x.StudentPercent,

////                            AgencyAmount =
////                                x.AgencyAmount,

////                            GajAmount =
////                                x.GajAmount,

////                            StudentAmount =
////                                x.StudentAmount,

////                            DebitAmount =
////                                x.DebitAmount,

////                            CreditAmount =
////                                x.CreditAmount,

////                            PersianExecutionDate =
////                                x.PersianExecutionDate
////                        })
////                    .ToList()
////        };
////    }

////    private sealed record SettlementGroupKey(
////        int PackageId,
////        int EducationalLevelId,
////        int ExamModeId,
////        int RegistrationPlanId,
////        int StudyFieldId,
////        int YearId,
////        int AgencyId,
////        string PersianExecutionDate);

////    private sealed class Plan5QuotaState
////    {
////        public Plan5QuotaState(
////            int freeQuotaCount,
////            int oneHundredThousandQuotaCount)
////        {
////            RemainingFreeQuota =
////                Math.Max(0, freeQuotaCount);

////            RemainingOneHundredThousandQuota =
////                Math.Max(0, oneHundredThousandQuotaCount);
////        }

////        public int RemainingFreeQuota { get; set; }

////        public int RemainingOneHundredThousandQuota { get; set; }
////    }

////    private sealed record GroupCalculation(
////        bool IsValid,
////        int FreeCandidateCount,
////        int PaidCandidateCount,
////        decimal UnitPrice,
////        decimal BaseAmount,
////        decimal AgencyPercent,
////        decimal GajPercent,
////        decimal StudentPercent,
////        decimal AgencyAmount,
////        decimal GajAmount,
////        decimal StudentAmount,
////        decimal DebitAmount,
////        decimal CreditAmount,
////        decimal TotalCreditGaj)
////    {
////        public static GroupCalculation Invalid()
////        {
////            return new GroupCalculation(
////                false,
////                0,
////                0,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m);
////        }

////        public static GroupCalculation Free(
////            int candidateCount,
////            decimal unitPrice)
////        {
////            return new GroupCalculation(
////                true,
////                candidateCount,
////                0,
////                unitPrice,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m,
////                0m);
////        }

////        public static GroupCalculation Debit(
////            int candidateCount,
////            int freeCandidateCount,
////            int paidCandidateCount,
////            decimal unitPrice,
////            decimal baseAmount)
////        {
////            return new GroupCalculation(
////                true,
////                freeCandidateCount,
////                paidCandidateCount,
////                unitPrice,
////                baseAmount,
////                0m,
////                0m,
////                0m,
////                baseAmount,
////                baseAmount,
////                0m,
////                baseAmount,
////                0m,
////                0m);
////        }

////        public static GroupCalculation Credit(
////            int candidateCount,
////            decimal unitPrice,
////            decimal baseAmount)
////        {
////            return new GroupCalculation(
////                true,
////                0,
////                candidateCount,
////                unitPrice,
////                baseAmount,
////                0m,
////                0m,
////                0m,
////                baseAmount,
////                baseAmount,
////                0m,
////                0m,
////                baseAmount,
////                0m);
////        }

////        public static GroupCalculation Regular(
////            int candidateCount,
////            decimal unitPrice,
////            decimal baseAmount,
////            decimal agencyPercent,
////            decimal gajPercent,
////            decimal studentPercent,
////            decimal agencyAmount,
////            decimal gajAmount,
////            decimal studentAmount)
////        {
////            return new GroupCalculation(
////                true,
////                0,
////                candidateCount,
////                unitPrice,
////                baseAmount,
////                agencyPercent,
////                gajPercent,
////                studentPercent,
////                gajAmount,
////                agencyAmount,
////                studentAmount,
////                agencyAmount,
////                0m,
////                gajAmount);
////        }

////        public static GroupCalculation Site(
////            int candidateCount,
////            decimal unitPrice,
////            decimal baseAmount,
////            decimal agencyPercent,
////            decimal gajPercent,
////            decimal studentPercent,
////            decimal agencyAmount,
////            decimal gajAmount,
////            decimal studentAmount)
////        {
////            return new GroupCalculation(
////                true,
////                0,
////                candidateCount,
////                unitPrice,
////                baseAmount,
////                agencyPercent,
////                gajPercent,
////                studentPercent,
////                gajAmount,
////                agencyAmount,
////                studentAmount,
////                0m,
////                agencyAmount,
////                0m);
////        }
////    }
////}
