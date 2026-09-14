using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Application.Common;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    public sealed class SettlementReportRepository : ISettlementReportRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementReportRepository(AgencySettlementDbContext context)
        {
            _context = context;
        }

        //    public async Task<SettlementReportResponse> GetReportAsync(
        //int? agencyId,
        //int yearId,
        //string persianExecutionDate,
        //int pageNumber,
        //int pageSize,
        //CancellationToken cancellationToken)
        //    {
        //        if (pageNumber < 1)
        //            pageNumber = 1;

        //        if (pageSize < 1)
        //            pageSize = 50;

        //        if (pageSize > 100)
        //            pageSize = 100;

        //        var settlementsQuery = _context.Settlements
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.YearId == yearId &&
        //                x.PersianExecutionDate == persianExecutionDate);

        //        if (agencyId.HasValue)
        //        {
        //            settlementsQuery = settlementsQuery
        //                .Where(x => x.AgencyId == agencyId.Value);
        //        }

        //        var settlementData = await settlementsQuery
        //            .GroupBy(x => x.AgencyId)
        //            .Select(g => new
        //            {
        //                AgencyId = g.Key,
        //                TotalDebit = g.Sum(x => x.TotalDebit),
        //                TotalCredit = g.Sum(x => x.TotalCredit)
        //            })
        //            .ToListAsync(cancellationToken);

        //        if (settlementData.Count == 0)
        //        {
        //            return new SettlementReportResponse
        //            {
        //                Items = new List<SettlementReportData>(),
        //                TotalCount = 0,
        //                PageNumber = pageNumber,
        //                PageSize = pageSize,
        //                TotalBookletCount = 0,
        //                TotalDebitAmount = 0m,
        //                TotalCreditAmount = 0m,
        //                TotalBalance = 0m
        //            };
        //        }

        //        var agencyIds = settlementData
        //            .Select(x => x.AgencyId)
        //            .Distinct()
        //            .ToList();

        //        var agencies = await _context.Agencies
        //            .AsNoTracking()
        //            .Where(x => agencyIds.Contains(x.Id))
        //            .Select(x => new
        //            {
        //                x.Id,
        //                x.DetailCode,
        //                x.Name
        //            })
        //            .ToListAsync(cancellationToken);

        //        var paymentData = await _context.Payments
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.YearId == yearId &&
        //                x.PersianExecutionDate == persianExecutionDate &&
        //                agencyIds.Contains(x.AgencyId))
        //            .GroupBy(x => x.AgencyId)
        //            .Select(g => new
        //            {
        //                AgencyId = g.Key,
        //                PaymentAmount = g.Sum(x => x.Amount)
        //            })
        //            .ToListAsync(cancellationToken);

        //        var bookletData = await _context.ExternalExamRecords
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.YearId == yearId &&
        //                x.PersianExecutionDate == persianExecutionDate &&
        //                x.CandidateExamId > 0 &&
        //                agencyIds.Contains(x.AgencyId))
        //            .GroupBy(x => x.AgencyId)
        //            .Select(g => new
        //            {
        //                AgencyId = g.Key,
        //                BookletCount = g
        //                    .Select(x => x.CandidateExamId)
        //                    .Distinct()
        //                    .Count()
        //            })
        //            .ToListAsync(cancellationToken);

        //        var agencyDictionary = agencies
        //            .ToDictionary(x => x.Id);

        //        var paymentDictionary = paymentData
        //            .ToDictionary(
        //                x => x.AgencyId,
        //                x => x.PaymentAmount);

        //        var bookletDictionary = bookletData
        //            .ToDictionary(
        //                x => x.AgencyId,
        //                x => x.BookletCount);

        //        var calculatedItems = settlementData
        //            .Select(settlement =>
        //            {
        //                var agency = agencyDictionary.TryGetValue(
        //                    settlement.AgencyId,
        //                    out var agencyData)
        //                    ? agencyData
        //                    : null;

        //                var paymentCredit = paymentDictionary.TryGetValue(
        //                    settlement.AgencyId,
        //                    out var payment)
        //                    ? payment
        //                    : 0m;

        //                var totalCredit =
        //                    settlement.TotalCredit + paymentCredit;

        //                var balance =
        //                    settlement.TotalDebit - totalCredit;

        //                var bookletCount = bookletDictionary.TryGetValue(
        //                    settlement.AgencyId,
        //                    out var booklet)
        //                    ? booklet
        //                    : 0;

        //                var status =
        //                    balance > 0
        //                        ? SettlementStatus.Debtor
        //                        : balance < 0
        //                            ? SettlementStatus.Creditor
        //                            : SettlementStatus.Settled;

        //                return new SettlementReportData
        //                {
        //                    AgencyId = settlement.AgencyId,
        //                    DetailCode = agency?.DetailCode ?? 0,
        //                    AgencyName = agency?.Name ?? string.Empty,
        //                    YearId = yearId,
        //                    PersianExecutionDate = persianExecutionDate,
        //                    BookletCount = bookletCount,
        //                    DebitAmount = settlement.TotalDebit,
        //                    CreditAmount = totalCredit,
        //                    Balance = balance,
        //                    Status = status
        //                };
        //            })
        //            .Where(x => x.Balance != 0)
        //            .OrderBy(x => x.AgencyName)
        //            .ToList();


        //        // ============================================================
        //        // Pagination فقط روی Items اعمال می‌شود.
        //        // جمع کل‌ها قبل از Pagination و بر اساس تمام رکوردها محاسبه می‌شوند.
        //        // ============================================================

        //        var totalCount = calculatedItems.Count;

        //        var totalBookletCount =
        //            calculatedItems.Sum(x => x.BookletCount);

        //        var totalDebitAmount =
        //            calculatedItems.Sum(x => x.DebitAmount);

        //        var totalCreditAmount =
        //            calculatedItems.Sum(x => x.CreditAmount);

        //        var totalBalance =
        //            calculatedItems.Sum(x => x.Balance);


        //        // فقط رکوردهای صفحه جاری برای Items
        //        var items = calculatedItems
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();


        //        // ============================================================
        //        // Total ها مربوط به تمام رکوردها هستند،
        //        // نه فقط رکوردهای صفحه جاری.
        //        // ============================================================

        //        return new SettlementReportResponse
        //        {
        //            Items = items,

        //            TotalCount = totalCount,
        //            PageNumber = pageNumber,
        //            PageSize = pageSize,

        //            // جمع کل تمام رکوردها
        //            TotalBookletCount = totalBookletCount,
        //            TotalDebitAmount = totalDebitAmount,
        //            TotalCreditAmount = totalCreditAmount,
        //            TotalBalance = totalBalance
        //        };
        //    }


        public async Task<SettlementReportResponse> GetReportAsync(
    int? agencyId,
    int yearId,
    string fromDate,
    string toDate,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 50;

            if (pageSize > 100)
                pageSize = 100;

            if (string.IsNullOrWhiteSpace(fromDate))
                throw new ArgumentException("تاریخ شروع وارد نشده است.");

            if (string.IsNullOrWhiteSpace(toDate))
                throw new ArgumentException("تاریخ پایان وارد نشده است.");

            var settlementsQuery = _context.Settlements
                .AsNoTracking()
                .Where(x =>
                    x.YearId == yearId &&
                    string.Compare(x.PersianExecutionDate, fromDate) >= 0 &&
                    string.Compare(x.PersianExecutionDate, toDate) <= 0);

            if (agencyId.HasValue)
            {
                settlementsQuery = settlementsQuery
                    .Where(x => x.AgencyId == agencyId.Value);
            }

            var settlementData = await settlementsQuery
                .Select(x => new
                {
                    SettlementId = x.Id,
                    AgencyId = x.AgencyId,
                    TotalDebit = x.TotalDebit,
                    TotalCredit = x.TotalCredit,
                    PersianExecutionDate = x.PersianExecutionDate
                })
                .ToListAsync(cancellationToken);

            if (settlementData.Count == 0)
            {
                return new SettlementReportResponse
                {
                    Items = [],
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalBookletCount = 0,
                    TotalDebitAmount = 0m,
                    TotalCreditAmount = 0m,
                    TotalBalance = 0m
                };
            }

            var agencyIds = settlementData
                .Select(x => x.AgencyId)
                .Distinct()
                .ToList();

            var agencies = await _context.Agencies
                .AsNoTracking()
                .Where(x => agencyIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.DetailCode,
                    x.Name
                })
                .ToListAsync(cancellationToken);

            var paymentData = await _context.Payments
                .AsNoTracking()
                .Where(x =>
                    x.YearId == yearId &&
                    string.Compare(x.PersianExecutionDate, fromDate) >= 0 &&
                    string.Compare(x.PersianExecutionDate, toDate) <= 0 &&
                    agencyIds.Contains(x.AgencyId))
                .GroupBy(x => x.AgencyId)
                .Select(g => new
                {
                    AgencyId = g.Key,
                    PaymentAmount = g.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);

            var bookletData = await _context.ExternalExamRecords
                .AsNoTracking()
                .Where(x =>
                    x.YearId == yearId &&
                    string.Compare(x.PersianExecutionDate, fromDate) >= 0 &&
                    string.Compare(x.PersianExecutionDate, toDate) <= 0 &&
                    x.CandidateExamId > 0 &&
                    agencyIds.Contains(x.AgencyId))
                .GroupBy(x => x.AgencyId)
                .Select(g => new
                {
                    AgencyId = g.Key,
                    BookletCount = g
                        .Select(x => x.CandidateExamId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync(cancellationToken);

            var agencyDictionary = agencies
                .ToDictionary(x => x.Id);

            var paymentDictionary = paymentData
                .ToDictionary(
                    x => x.AgencyId,
                    x => x.PaymentAmount);

            var bookletDictionary = bookletData
                .ToDictionary(
                    x => x.AgencyId,
                    x => x.BookletCount);

            var calculatedItems = settlementData
                .Select(settlement =>
                {
                    var agency = agencyDictionary.TryGetValue(
                        settlement.AgencyId,
                        out var agencyData)
                        ? agencyData
                        : null;

                    var paymentCredit = paymentDictionary.TryGetValue(
                        settlement.AgencyId,
                        out var payment)
                        ? payment
                        : 0m;

                    var totalCredit =
                        settlement.TotalCredit + paymentCredit;

                    var balance =
                        settlement.TotalDebit - totalCredit;

                    var bookletCount = bookletDictionary.TryGetValue(
                        settlement.AgencyId,
                        out var booklet)
                        ? booklet
                        : 0;

                    var status =
                        balance > 0
                            ? SettlementStatus.Debtor
                            : balance < 0
                                ? SettlementStatus.Creditor
                                : SettlementStatus.Settled;

                    return new SettlementReportData
                    {
                        SettlementId = settlement.SettlementId,

                        AgencyId = settlement.AgencyId,

                        DetailCode =
                            agency?.DetailCode ?? 0,

                        AgencyName =
                            agency?.Name ?? string.Empty,

                        YearId = yearId,

                        PersianExecutionDate =
                            settlement.PersianExecutionDate,

                        BookletCount =
                            bookletCount,

                        DebitAmount =
                            settlement.TotalDebit,

                        CreditAmount =
                            totalCredit,

                        Balance =
                            balance,

                        Status =
                            status
                    };
                })
                .Where(x => x.Balance != 0)
                .OrderByDescending(x => GetPersianDateSortValue(x.PersianExecutionDate))
                .ThenBy(x => x.AgencyName)
                .ToList();

            var totalCount =
                calculatedItems.Count;

            var totalBookletCount =
                calculatedItems.Sum(x => x.BookletCount);

            var totalDebitAmount =
                calculatedItems.Sum(x => x.DebitAmount);

            var totalCreditAmount =
                calculatedItems.Sum(x => x.CreditAmount);

            var totalBalance =
                calculatedItems.Sum(x => x.Balance);

            var items = calculatedItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new SettlementReportResponse
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalBookletCount = totalBookletCount,
                TotalDebitAmount = totalDebitAmount,
                TotalCreditAmount = totalCreditAmount,
                TotalBalance = totalBalance
            };
        }

        private static long GetPersianDateSortValue(string date)
        {
            var parts = date
                .Trim()
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
                return 0;

            if (!int.TryParse(parts[0], out var year))
                return 0;

            if (!int.TryParse(parts[1], out var month))
                return 0;

            if (!int.TryParse(parts[2], out var day))
                return 0;

            return (long)year * 10000 + month * 100 + day;
        }
        //   public async Task<List<SettlementDetailItemDto>> GetSettlementDetailsAsync(
        //int agencyId,
        //int yearId,
        //string persianExecutionDate,
        //CancellationToken cancellationToken)
        //   {
        //       var items = await (
        //           from item in _context.SettlementItems

        //           join package in _context.Packages
        //               on item.PackageId equals package.Id

        //           join educationalLevel in _context.EducationalLevels
        //               on item.EducationalLevelId equals educationalLevel.Id

        //           join studyField in _context.StudyFields
        //               on item.StudyFieldId equals studyField.Id

        //           join examMode in _context.ExamModes
        //               on item.ExamModeId equals examMode.Id

        //               join agencies in _context.Agencies 
        //               on item.AgencyId equals agencies.Id

        //               join yearTypes in _context.YearTypes
        //               on item.YearId equals yearTypes.Id

        //               join registrationPlan in _context.RegistrationPlans
        //               on item.RegistrationPlanId equals registrationPlan.Id

        //           where item.AgencyId == agencyId
        //                 && item.YearId == yearId
        //                 && item.PersianExecutionDate == persianExecutionDate

        //           orderby item.EducationalLevelId, item.StudyFieldId

        //           select new SettlementDetailItemDto
        //           {
        //               SettlementItemId = item.Id,

        //               PackageId = item.PackageId,
        //               PackageName = package.Name,

        //               ExamModeId = item.ExamModeId,
        //               ExamModeName = examMode.Name,
        //               AgencyName = agencies.Name,
        //               YearName = yearTypes.Name,
        //               EducationalLevelId = item.EducationalLevelId,
        //               EducationalLevelName = educationalLevel.Name,
        //               RegistrationPlanId = registrationPlan.Id,
        //               RegistrationPlanName = registrationPlan.Name,
        //               StudyFieldId = item.StudyFieldId,
        //               StudyFieldName = studyField.Name,

        //               CandidateCount = item.CandidateCount,
        //               FreeCandidateCount = item.FreeCandidateCount,
        //               PaidCandidateCount = item.PaidCandidateCount,

        //               UnitPrice = item.UnitPrice,
        //               BaseAmount = item.BaseAmount,

        //               DiscountPercent = item.GajPercent,

        //               DiscountAmount =
        //                   item.BaseAmount * item.GajPercent / 100m,

        //               AgencyPercent = item.AgencyPercent,

        //               AgencyAmount =
        //                   item.BaseAmount * item.AgencyPercent / 100m,

        //               CreditAmount =
        //                   item.BaseAmount * item.AgencyPercent / 100m,

        //               TotalAmount =
        //                   item.BaseAmount -
        //                   (item.BaseAmount * item.GajPercent / 100m)
        //           }
        //       ).ToListAsync(cancellationToken);

        //       foreach (var item in items)
        //       {
        //           item.StageTypeId = GetStageTypeId(
        //               item.EducationalLevelId);

        //           item.StageTypeName = GetStageTypeName(
        //               item.EducationalLevelId);

        //           item.Title =
        //               $"{item.ExamModeName} - " +
        //               $"{item.StageTypeName} - " +
        //               $"پایه {item.EducationalLevelName} - " +
        //               $"رشته {item.StudyFieldName}";
        //       }

        //       return items;
        //   }


        public async Task<SettlementDetailsDto> GetSettlementDetailsAsync(
       int agencyId,
       int yearId,
       string persianExecutionDate,
       CancellationToken cancellationToken)
        {
            var items = await (
                from item in _context.SettlementItems.AsNoTracking()
                join package in _context.Packages
                    on item.PackageId equals package.Id
                join educationalLevel in _context.EducationalLevels
                    on item.EducationalLevelId equals educationalLevel.Id
                join studyField in _context.StudyFields
                    on item.StudyFieldId equals studyField.Id
                join examMode in _context.ExamModes
                    on item.ExamModeId equals examMode.Id
                join agency in _context.Agencies
                    on item.AgencyId equals agency.Id
                join yearType in _context.YearTypes
                    on item.YearId equals yearType.Id
                join registrationPlan in _context.RegistrationPlans
                    on item.RegistrationPlanId equals registrationPlan.Id
                where item.AgencyId == agencyId
                      && item.YearId == yearId
                      && item.PersianExecutionDate == persianExecutionDate
                orderby item.ExamModeId, item.EducationalLevelId, item.StudyFieldId
                select new SettlementDetailItemDto
                {
                    SettlementItemId = item.Id,
                    PackageId = item.PackageId,
                    PackageName = package.Name,

                    ExamModeId = item.ExamModeId,
                    ExamModeName = examMode.Name,

                    AgencyName = agency.Name,
                    YearName = yearType.Name,

                    EducationalLevelId = item.EducationalLevelId,
                    EducationalLevelName = educationalLevel.Name,

                    RegistrationPlanId = item.RegistrationPlanId,
                    RegistrationPlanName = registrationPlan.Name,

                    StudyFieldId = item.StudyFieldId,
                    StudyFieldName = studyField.Name,

                    CandidateCount = item.CandidateCount,
                    FreeCandidateCount = item.FreeCandidateCount,
                    PaidCandidateCount = item.PaidCandidateCount,

                    UnitPrice = item.UnitPrice,
                    BaseAmount = item.BaseAmount,

                    DiscountPercent = item.GajPercent,
                    DiscountAmount = item.BaseAmount * item.GajPercent / 100m,

                    AgencyPercent = item.AgencyPercent,
                    AgencyAmount = item.AgencyAmount,
                    GajAmount = item.GajAmount,
                    StudentAmount = item.StudentAmount,

                    DebitAmount = item.DebitAmount,
                    CreditAmount = item.CreditAmount,

                    TotalAmount = item.BaseAmount -
                                  (item.BaseAmount * item.GajPercent / 100m)
                }
            ).ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                item.StageTypeId = GetStageTypeId(item.EducationalLevelId);
                item.StageTypeName = GetStageTypeName(item.EducationalLevelId);

                item.Title =
                    $"{item.ExamModeName} - " +
                    $"{item.StageTypeName} - " +
                    $"پایه {item.EducationalLevelName} - " +
                    $"رشته {item.StudyFieldName}";
            }

            var inPersonItems = items;

            var onlineItems = items
                ;

            var totalStudentFree = items;

            return new SettlementDetailsDto
            {
                Items = items,

                TotalBaseAmount = items.Sum(x => x.BaseAmount),

                //حضوری و آزاد

                TotalInPersonFreeAgencyDebit = inPersonItems
                    .Where(x => x.RegistrationPlanId == 1 && x.ExamModeId == 0)
                    .Sum(x => x.DebitAmount),

                TotalInPersonFreeGajCredit = inPersonItems
                    .Where(x => x.RegistrationPlanId == 1 && x.ExamModeId == 0)
                    .Sum(x => x.AgencyAmount),

                TotalStudentCountFree = items
    .Where(x => x.RegistrationPlanId == 1 && x.ExamModeId == 0)
    .Sum(x => x.CandidateCount),


                //حکمت حضوری

                TotalInPersonHekmatAgencyCredit = inPersonItems
                    .Where(x => x.RegistrationPlanId == 2 && x.ExamModeId == 0)
                    .Sum(x => x.CreditAmount),

                TotalInPersonHekmatGajDebit = inPersonItems
                    .Where(x => x.RegistrationPlanId == 2 && x.ExamModeId == 0)
                    .Sum(x => x.GajAmount),

                TotalStudentPersonHekmat = items
    .Where(x => x.RegistrationPlanId == 2 && x.ExamModeId == 0)
    .Sum(x => x.CandidateCount),

                // سایت حضوری // // //

                TotalInPersonSiteAgencyCredit = inPersonItems
                    .Where(x => x.RegistrationPlanId == 8 && x.ExamModeId == 0)
                    .Sum(x => x.AgencyAmount),

                TotalInPersonSiteGajDebit = inPersonItems
                    .Where(x => x.RegistrationPlanId == 8 && x.ExamModeId == 0)
                    .Sum(x => x.GajAmount),
                TotalStudentPersonSite = items.Where(x => x.RegistrationPlanId == 8 && x.ExamModeId == 0)
                    .Sum(x => x.CandidateCount),



                // سهمیه ای حضوری
                TotalInPersonScholarshipAgencyDebit = inPersonItems
    .Where(x => (x.RegistrationPlanId == 3 || x.RegistrationPlanId == 5)
             && x.ExamModeId == 0)
    .Sum(x => x.DebitAmount),

                TotalInPersonScholarshipGajCredit = inPersonItems
                    .Where(x => (x.RegistrationPlanId == 3 || x.RegistrationPlanId == 5) && x.ExamModeId == 0)
                    .Sum(x => x.CreditAmount),
                TotalStudentPersonScholarship = items.Where(x => (x.RegistrationPlanId == 3 || x.RegistrationPlanId == 5) && x.ExamModeId == 0)
                    .Sum(x => x.CandidateCount),



                // آزاد حضوری
                TotalOnlineFreeAgencyDebit = onlineItems
                    .Where(x => x.RegistrationPlanId == 1 && x.ExamModeId == 1)
                    .Sum(x => x.DebitAmount),

                TotalOnlineFreeGajCredit = onlineItems
                    .Where(x => x.RegistrationPlanId == 1 && x.ExamModeId == 1)
                    .Sum(x => x.AgencyAmount),
                TotalStudentOnlineFree = items.Where(x=> x.RegistrationPlanId == 1 && x.ExamModeId == 1)
                .Sum(x=> x.CandidateCount),



                // حکمت حضوری
                TotalOnlineHekmatAgencyCredit = onlineItems
                    .Where(x => x.RegistrationPlanId == 2 && x.ExamModeId == 1)
                    .Sum(x => x.CreditAmount),

                TotalOnlineHekmatGajDebit = onlineItems
                    .Where(x => x.RegistrationPlanId == 2 && x.ExamModeId == 1)
                    .Sum(x => x.GajAmount),
                TotalStudentOnlineHekmat = items.Where(x=> x.RegistrationPlanId == 2 && x.ExamModeId == 1)
                .Sum(x=> x.CandidateCount),

                //ثبت نام از سایت
                TotalOnlineSiteAgencyCredit = onlineItems
                    .Where(x => x.RegistrationPlanId == 8 && x.ExamModeId == 1)
                    .Sum(x => x.AgencyAmount),

                TotalOnlineSiteGajDebit = onlineItems
                    .Where(x => x.RegistrationPlanId == 8 && x.ExamModeId == 1)
                    .Sum(x => x.GajAmount),
                TotalStudentOnlineSite = items.Where(x=> x.RegistrationPlanId == 8 && x.ExamModeId == 1)
                .Sum(x=> x.CandidateCount),


                TotalOnlineScholarshipAgencyCredit = onlineItems
                .Where(x=> (x.RegistrationPlanId == 3 || x.RegistrationPlanId == 5) && x.ExamModeId == 1)
                .Sum(x=> x.AgencyAmount),
                TotalOnlineScholarshipGajCredit = onlineItems.
                Where(x=> (x.RegistrationPlanId == 3 || x.RegistrationPlanId == 5)
                && x.ExamModeId == 1)
                .Sum(x=> x.GajAmount),
                TotalStudentOnlineScholarship = items.
                Where(x=> (x.RegistrationPlanId == 3 || x.RegistrationPlanId == 5) && x.ExamModeId == 1)
                .Sum(x=> x.CandidateCount),

                // دانش آموز
                TotalOnlineStudentAmount = onlineItems
                    .Sum(x => x.StudentAmount)
            }; 
        }


        private static int GetStageTypeId(int educationalLevelId)
            => educationalLevelId switch
            {
                >= 1 and <= 6 => 1,
                >= 7 and <= 9 => 2,
                >= 10 and <= 12 => 3,
                _ => 0
            };

        private static string GetStageTypeName(int educationalLevelId)
            => educationalLevelId switch
            {
                >= 1 and <= 6 => "ابتدایی توصیفی",
                >= 7 and <= 9 => "متوسطه اول",
                >= 10 and <= 12 => "متوسطه دوم",
                _ => string.Empty
            };



        public async Task<SettlementSelectionDetailsDto> GetSettlementSelectionDetailsAsync(
       int agencyId,
       long[] settlementIds,
       CancellationToken cancellationToken)
        {
            var items = await (
                from item in _context.SettlementItems.AsNoTracking()

                join package in _context.Packages.AsNoTracking()
                    on item.PackageId equals package.Id

                join educationalLevel in _context.EducationalLevels.AsNoTracking()
                    on item.EducationalLevelId equals educationalLevel.Id

                join studyField in _context.StudyFields.AsNoTracking()
                    on item.StudyFieldId equals studyField.Id

                join examMode in _context.ExamModes.AsNoTracking()
                    on item.ExamModeId equals examMode.Id

                join agency in _context.Agencies.AsNoTracking()
                    on item.AgencyId equals agency.Id

                join yearType in _context.YearTypes.AsNoTracking()
                    on item.YearId equals yearType.Id

                join registrationPlan in _context.RegistrationPlans.AsNoTracking()
                    on item.RegistrationPlanId equals registrationPlan.Id

                where item.AgencyId == agencyId
                      && settlementIds.Contains(item.SettlementId)

                select new SettlementSelectionDetailItemDto
                {
                    SettlementId = item.SettlementId,

                    SettlementItemId = item.Id,

                    PackageId = item.PackageId,
                    PackageName = package.Name,

                    ExamModeId = item.ExamModeId,
                    ExamModeName = examMode.Name,

                    AgencyName = agency.Name,

                    YearId = item.YearId,
                    YearName = yearType.Name,

                    EducationalLevelId = item.EducationalLevelId,
                    EducationalLevelName = educationalLevel.Name,

                    RegistrationPlanId = item.RegistrationPlanId,
                    RegistrationPlanName = registrationPlan.Name,

                    StudyFieldId = item.StudyFieldId,
                    StudyFieldName = studyField.Name,

                    PersianExecutionDate = item.PersianExecutionDate,

                    CandidateCount = item.CandidateCount,
                    FreeCandidateCount = item.FreeCandidateCount,
                    PaidCandidateCount = item.PaidCandidateCount,

                    UnitPrice = item.UnitPrice,
                    BaseAmount = item.BaseAmount,

                    DiscountPercent = item.GajPercent,

                    DiscountAmount =
                        item.BaseAmount *
                        item.GajPercent /
                        100m,

                    AgencyPercent = item.AgencyPercent,

                    AgencyAmount = item.AgencyAmount,

                    GajAmount = item.GajAmount,

                    StudentAmount = item.StudentAmount,

                    DebitAmount = item.DebitAmount,

                    CreditAmount = item.CreditAmount,

                    TotalAmount =
                        item.BaseAmount -
                        (
                            item.BaseAmount *
                            item.GajPercent /
                            100m
                        )
                }
            ).ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                item.StageTypeId =
                    GetStageTypeId(item.EducationalLevelId);

                item.StageTypeName =
                    GetStageTypeName(item.EducationalLevelId);

                item.Title =
                    $"{item.ExamModeName} - " +
                    $"{item.StageTypeName} - " +
                    $"پایه {item.EducationalLevelName} - " +
                    $"رشته {item.StudyFieldName}";
            }

            items = items
                .OrderBy(x => GetPersianDateSortValue(
                    x.PersianExecutionDate))
                .ThenBy(x => x.ExamModeId)
                .ThenBy(x => x.EducationalLevelId)
                .ThenBy(x => x.StudyFieldId)
                .ToList();

            var inPersonItems = items
                .Where(x => x.ExamModeId == 0)
                .ToList();

            var onlineItems = items
                .Where(x => x.ExamModeId == 1)
                .ToList();

            return new SettlementSelectionDetailsDto
            {
                Items = items,

                TotalBaseAmount =
                    items.Sum(x => x.BaseAmount),

                TotalStudentCountFree =
                    items
                        .Where(x =>
                            x.RegistrationPlanId == 1 &&
                            x.ExamModeId == 0)
                        .Sum(x => x.CandidateCount),

                TotalInPersonFreeAgencyDebit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 1)
                        .Sum(x => x.DebitAmount),

                TotalInPersonFreeGajCredit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 1)
                        .Sum(x => x.AgencyAmount),

                TotalOnlineStudentAmount =
                    onlineItems.Sum(x => x.StudentAmount),

                TotalInPersonHekmatAgencyCredit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 2)
                        .Sum(x => x.CreditAmount),

                TotalInPersonHekmatGajDebit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 2)
                        .Sum(x => x.GajAmount),

                TotalStudentPersonHekmat =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 2)
                        .Sum(x => x.CandidateCount),

                TotalInPersonSiteAgencyCredit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 8)
                        .Sum(x => x.AgencyAmount),

                TotalInPersonSiteGajDebit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 8)
                        .Sum(x => x.GajAmount),

                TotalStudentPersonSite =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 8)
                        .Sum(x => x.CandidateCount),

                TotalInPersonScholarshipAgencyDebit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 3 ||
                            x.RegistrationPlanId == 5)
                        .Sum(x => x.DebitAmount),

                TotalInPersonScholarshipGajCredit =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 3 ||
                            x.RegistrationPlanId == 5)
                        .Sum(x => x.CreditAmount),

                TotalStudentPersonScholarship =
                    inPersonItems
                        .Where(x =>
                            x.RegistrationPlanId == 3 ||
                            x.RegistrationPlanId == 5)
                        .Sum(x => x.CandidateCount),

                TotalOnlineFreeAgencyDebit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 1)
                        .Sum(x => x.DebitAmount),

                TotalOnlineFreeGajCredit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 1)
                        .Sum(x => x.AgencyAmount),

                TotalStudentOnlineFree =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 1)
                        .Sum(x => x.CandidateCount),

                TotalOnlineHekmatAgencyCredit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 2)
                        .Sum(x => x.CreditAmount),

                TotalOnlineHekmatGajDebit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 2)
                        .Sum(x => x.GajAmount),

                TotalStudentOnlineHekmat =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 2)
                        .Sum(x => x.CandidateCount),

                TotalOnlineSiteAgencyCredit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 8)
                        .Sum(x => x.AgencyAmount),

                TotalOnlineSiteGajDebit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 8)
                        .Sum(x => x.GajAmount),

                TotalStudentOnlineSite =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 8)
                        .Sum(x => x.CandidateCount),

                TotalOnlineScholarshipAgencyCredit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 3 ||
                            x.RegistrationPlanId == 5)
                        .Sum(x => x.AgencyAmount),

                TotalOnlineScholarshipGajCredit =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 3 ||
                            x.RegistrationPlanId == 5)
                        .Sum(x => x.GajAmount),

                TotalStudentOnlineScholarship =
                    onlineItems
                        .Where(x =>
                            x.RegistrationPlanId == 3 ||
                            x.RegistrationPlanId == 5)
                        .Sum(x => x.CandidateCount)
            };
        }

    }
}