using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.ExternalDtos;
using AgencySettlement.Application.DTOs.SingleLessonDto;
using AgencySettlement.Domain.Entities;
using AgencySettlement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AgencySettlement.Infrastructure.Persistence.Repositories;

public sealed class ExternalSettlementStatusRepository
  : IExternalSettlementStatusRepository
{
    private readonly AgencySettlementDbContext _context;

    public ExternalSettlementStatusRepository(
        AgencySettlementDbContext context)
    {
        _context = context;
    }


    public async Task<ExternalSettlementStatusDto?> GetExternalSettlementStatusAsync(
    int agencyId,
    int yearId,
    string persianExecutionDate,
    CancellationToken cancellationToken)
    {
        // -----------------------------------------
        // Exam settlement
        // -----------------------------------------

        var settlement = await _context.Settlements
            .AsNoTracking()
            .Where(x =>
                x.AgencyId == agencyId &&
                x.YearId == yearId &&
                x.PersianExecutionDate == persianExecutionDate)
            .OrderByDescending(x => x.Id)
            .Select(x => new
            {
                x.Id,
                x.AgencyId,
                x.Balance,
                x.YearId,
                x.PersianExecutionDate,
                x.TotalDebit
            })
            .FirstOrDefaultAsync(cancellationToken);

        // -----------------------------------------
        // Single lesson settlement
        // -----------------------------------------

        var singleLessonSettlement =
            await _context.SingleLessonSettlements
                .AsNoTracking()
                .Where(x =>
                    x.AgencyId == agencyId &&
                    x.YearId == yearId &&
                    x.PersianExecutionDate == persianExecutionDate)
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.TotalDebit,
                    x.TotalCourseCount
                })
                .FirstOrDefaultAsync(cancellationToken);

        // اگر هیچ‌کدام از تسویه‌ها وجود نداشته باشند،
        // وضعیت تسویه‌ای برای برگرداندن نداریم.
        if (settlement == null && singleLessonSettlement == null)
            return null;

        // -----------------------------------------
        // Exam items
        // منطق و محاسبات قبلی آزمون حفظ شده است.
        // -----------------------------------------

        var items = await (
            from item in _context.SettlementOrderItems.AsNoTracking()

            join order in _context.SettlementOrders.AsNoTracking()
                on item.SettlementOrderId equals order.Id

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
                  && item.YearId == yearId
                  && item.PersianExecutionDate == persianExecutionDate

            orderby item.ExamModeId,
                    item.EducationalLevelId,
                    item.StudyFieldId

            select new SettlementDetailItemDto
            {
                SettlementItemId = item.Id,
                SettlementId = order.Id,

                PackageId = item.PackageId,
                PackageName = package.Name,

                ExamModeId = item.ExamModeId,
                ExamModeName = examMode.Name,

                AgencyName = agency.Name,
                YearName = yearType.Name,
                DetailCode = agency.DetailCode,

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

                DiscountAmount =
                    item.BaseAmount * item.GajPercent / 100m,

                AgencyPercent = item.AgencyPercent,

                AgencyAmount = item.AgencyAmount,
                GajAmount = item.GajAmount,
                StudentAmount = item.StudentAmount,

                DebitAmount = item.DebitAmount,
                CreditAmount = item.CreditAmount,

                TotalAmount =
                    item.BaseAmount -
                    (item.BaseAmount * item.GajPercent / 100m)
            }
        ).ToListAsync(cancellationToken);

        // همان محاسبه قبلی عنوان ردیف‌های آزمون
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

        // همان تعدادهای قبلی آزمون؛ تغییری در منطق نیست.
        var totalInPersonCount = items
            .Where(x => x.ExamModeId == 0)
            .Sum(x => x.CandidateCount);

        var totalOnlineCount = items
            .Where(x => x.ExamModeId == 1)
            .Sum(x => x.CandidateCount);

        // -----------------------------------------
        // Single lesson items
        // -----------------------------------------

        var singleLessonRows = await (
     from item in _context.SingleLessonSettlementItems.AsNoTracking()

     join package in _context.Packages.AsNoTracking()
         on item.PackageId equals package.Id

     join educationalLevel in _context.EducationalLevels.AsNoTracking()
         on item.EducationalLevelId equals educationalLevel.Id

     join studyField in _context.StudyFields.AsNoTracking()
         on item.StudyFieldId equals studyField.Id

     join examMode in _context.ExamModes.AsNoTracking()
         on item.ExamModeId equals examMode.Id

     join registrationPlan in _context.RegistrationPlans.AsNoTracking()
         on item.RegistrationPlanId equals registrationPlan.Id

     where item.AgencyId == agencyId
           && item.YearId == yearId
           && item.PersianExecutionDate == persianExecutionDate

     select new SingleLessonSettlementDetailItemDto
     {
         // شناسه رکورد دیگر مبنای تجمیع نیست.
         SettlementId = item.SettlementId,

         CourseId = item.CourseId,

         CourseName = _context.SingleLessons
             .Where(course =>
                 course.YearId == item.YearId &&
                 course.CourseId == item.CourseId &&
                 course.StudyFieldId == item.StudyFieldId &&
                 course.EducationalLevelId == item.EducationalLevelId &&
                 course.PackageId == item.PackageId)
             .Select(course => course.CourseName)
             .FirstOrDefault() ?? string.Empty,

         PackageId = item.PackageId,
         PackageName = package.Name,

         EducationalLevelId = item.EducationalLevelId,
         EducationalLevelName = educationalLevel.Name,

         StudyFieldId = item.StudyFieldId,
         StudyFieldName = studyField.Name,

         ExamModeId = item.ExamModeId,
         ExamModeName = examMode.Name,

         RegistrationPlanId = item.RegistrationPlanId,
         RegistrationPlanName = registrationPlan.Name,

         Quantity = item.Quantity,

         UnitPrice = item.UnitPrice,
         BaseAmount = item.BaseAmount,

         AgencyPercent = item.AgencyPercent,
         GajPercent = item.GajPercent,

         AgencyAmount = item.AgencyAmount,
         GajAmount = item.GajAmount,

         DebitAmount = item.DebitAmount
     }
 ).ToListAsync(cancellationToken);

        // تجمیع رکوردهای همسان
        var singleLessonItems = singleLessonRows
            .GroupBy(item => new
            {
                item.SettlementId,

                item.CourseId,
                item.CourseName,

                item.PackageId,
                item.PackageName,

                item.EducationalLevelId,
                item.EducationalLevelName,

                item.StudyFieldId,
                item.StudyFieldName,

                item.ExamModeId,
                item.ExamModeName,

                item.RegistrationPlanId,
                item.RegistrationPlanName,

                // جلوگیری از ادغام ردیف‌هایی با شرایط مالی متفاوت
                item.UnitPrice,
                item.AgencyPercent,
                item.GajPercent
            })
            .Select(group => new SingleLessonSettlementDetailItemDto
            {
                SettlementId = group.Key.SettlementId,

                CourseId = group.Key.CourseId,
                CourseName = group.Key.CourseName,

                PackageId = group.Key.PackageId,
                PackageName = group.Key.PackageName,

                EducationalLevelId = group.Key.EducationalLevelId,
                EducationalLevelName = group.Key.EducationalLevelName,

                StudyFieldId = group.Key.StudyFieldId,
                StudyFieldName = group.Key.StudyFieldName,

                ExamModeId = group.Key.ExamModeId,
                ExamModeName = group.Key.ExamModeName,

                RegistrationPlanId = group.Key.RegistrationPlanId,
                RegistrationPlanName = group.Key.RegistrationPlanName,

                Quantity = group.Sum(item => item.Quantity),

                UnitPrice = group.Key.UnitPrice,

                BaseAmount = group.Sum(item => item.BaseAmount),

                AgencyPercent = group.Key.AgencyPercent,
                GajPercent = group.Key.GajPercent,

                AgencyAmount = group.Sum(item => item.AgencyAmount),
                GajAmount = group.Sum(item => item.GajAmount),

                DebitAmount = group.Sum(item => item.DebitAmount)
            })
            .OrderBy(item => item.EducationalLevelId)
            .ThenBy(item => item.StudyFieldId)
            .ThenBy(item => item.CourseId)
            .ThenBy(item => item.PackageId)
            .ThenBy(item => item.ExamModeId)
            .ThenBy(item => item.RegistrationPlanId)
            .ToList();

        // -----------------------------------------
        // Debt summaries
        // -----------------------------------------

        var examTotalDebit = settlement?.TotalDebit ?? 0m;

        var singleLessonTotalDebit =
            singleLessonSettlement?.TotalDebit ?? 0m;

        var totalDebit =
            examTotalDebit + singleLessonTotalDebit;

        // تعداد تک‌درس از ردیف‌های واقعی محاسبه می‌شود.
        var singleLessonTotalCourseCount =
            singleLessonItems.Sum(x => x.Quantity);

        // -----------------------------------------
        // Payment status
        // -----------------------------------------

        var paymentStartDate =
            settlement?.PersianExecutionDate ?? persianExecutionDate;

        var paymentDeadlineDate = AddDays(
            paymentStartDate,
            4);

        var paymentStatus = CalculateStatus(
            paymentStartDate,
            paymentDeadlineDate);

        // -----------------------------------------
        // Response
        // -----------------------------------------

        return new ExternalSettlementStatusDto
        {
            AgencyId = agencyId,
            YearId = yearId,

            SettlmentId = settlement?.Id ?? 0L,

            SingleLessonSettlementId =
                singleLessonSettlement?.Id,

            PersianExecutionDate = paymentStartDate,
            PaymentDeadline = paymentDeadlineDate,
            PaymentStatus = paymentStatus,

            // فیلد قدیمی؛ مقدار آزمون حفظ می‌شود.
            Balance = settlement?.Balance ?? 0m,

            // جزئیات آزمون
            Items = items,
            TotalInPersonCount = totalInPersonCount,
            TotalOnlineCount = totalOnlineCount,

            // جزئیات تک‌درس
            SingleLessonItems = singleLessonItems,

            // فقط بدهکاری‌ها
            ExamTotalDebit = examTotalDebit,
            SingleLessonTotalDebit = singleLessonTotalDebit,
            TotalDebit = totalDebit,

            // تعداد تک‌درس
            SingleLessonTotalCourseCount =
                singleLessonTotalCourseCount
        };
    }
    //public async Task<ExternalSettlementStatusDto?> GetExternalSettlementStatusAsync(
    //int agencyId,
    //int yearId,
    //string persianExecutionDate,
    //CancellationToken cancellationToken)
    //{
    //    var settlement = await _context.Settlements
    //        .AsNoTracking()
    //        .Where(x =>
    //            x.AgencyId == agencyId &&
    //            x.YearId == yearId &&
    //            x.PersianExecutionDate == persianExecutionDate)
    //        .OrderByDescending(x => x.Id)
    //        .Select(x => new
    //        {
    //            x.Id,
    //            x.AgencyId,
    //            x.Balance,
    //            x.YearId,
    //            x.PersianExecutionDate,
    //            x.TotalDebit
    //        })
    //        .FirstOrDefaultAsync(cancellationToken);

    //    var databaseName = _context.Database.GetDbConnection().Database;
    //    var serverName = _context.Database.GetDbConnection().DataSource;

    //    if (settlement == null)
    //        return null;

    //    // -----------------------------------------
    //    // Items
    //    // -----------------------------------------

    //    var items = await (
    //        from item in _context.SettlementOrderItems.AsNoTracking()

    //        join order in _context.SettlementOrders.AsNoTracking()
    //            on item.SettlementOrderId equals order.Id

    //        join package in _context.Packages.AsNoTracking()
    //            on item.PackageId equals package.Id

    //        join educationalLevel in _context.EducationalLevels.AsNoTracking()
    //            on item.EducationalLevelId equals educationalLevel.Id

    //        join studyField in _context.StudyFields.AsNoTracking()
    //            on item.StudyFieldId equals studyField.Id

    //        join examMode in _context.ExamModes.AsNoTracking()
    //            on item.ExamModeId equals examMode.Id

    //        join agency in _context.Agencies.AsNoTracking()
    //            on item.AgencyId equals agency.Id

    //        join yearType in _context.YearTypes.AsNoTracking()
    //            on item.YearId equals yearType.Id

    //        join registrationPlan in _context.RegistrationPlans.AsNoTracking()
    //            on item.RegistrationPlanId equals registrationPlan.Id

    //        where item.AgencyId == agencyId
    //              && item.YearId == yearId
    //              && item.PersianExecutionDate == persianExecutionDate

    //        orderby item.ExamModeId,
    //                item.EducationalLevelId,
    //                item.StudyFieldId

    //        select new SettlementDetailItemDto
    //        {
    //            SettlementItemId = item.Id,

    //            SettlementId = order.Id,

    //            PackageId = item.PackageId,
    //            PackageName = package.Name,

    //            ExamModeId = item.ExamModeId,
    //            ExamModeName = examMode.Name,

    //            AgencyName = agency.Name,
    //            YearName = yearType.Name,
    //            DetailCode = agency.DetailCode,

    //            EducationalLevelId = item.EducationalLevelId,
    //            EducationalLevelName = educationalLevel.Name,

    //            RegistrationPlanId = item.RegistrationPlanId,
    //            RegistrationPlanName = registrationPlan.Name,

    //            StudyFieldId = item.StudyFieldId,
    //            StudyFieldName = studyField.Name,

    //            CandidateCount = item.CandidateCount,
    //            FreeCandidateCount = item.FreeCandidateCount,
    //            PaidCandidateCount = item.PaidCandidateCount,

    //            UnitPrice = item.UnitPrice,
    //            BaseAmount = item.BaseAmount,

    //            DiscountPercent = item.GajPercent,

    //            DiscountAmount =
    //                item.BaseAmount *
    //                item.GajPercent /
    //                100m,

    //            AgencyPercent = item.AgencyPercent,

    //            AgencyAmount = item.AgencyAmount,
    //            GajAmount = item.GajAmount,
    //            StudentAmount = item.StudentAmount,

    //            DebitAmount = item.DebitAmount,
    //            CreditAmount = item.CreditAmount,

    //            TotalAmount =
    //                item.BaseAmount -
    //                (
    //                    item.BaseAmount *
    //                    item.GajPercent /
    //                    100m
    //                )
    //        }
    //    ).ToListAsync(cancellationToken);

    //    foreach (var item in items)
    //    {
    //        item.StageTypeId =
    //            GetStageTypeId(item.EducationalLevelId);

    //        item.StageTypeName =
    //            GetStageTypeName(item.EducationalLevelId);

    //        item.Title =
    //            $"{item.ExamModeName} - " +
    //            $"{item.StageTypeName} - " +
    //            $"پایه {item.EducationalLevelName} - " +
    //            $"رشته {item.StudyFieldName}";
    //    }

    //    var totalInPersonCount = items
    //        .Where(x => x.ExamModeId == 0)
    //        .Sum(x => x.CandidateCount);

    //    var totalOnlineCount = items
    //        .Where(x => x.ExamModeId == 1)
    //        .Sum(x => x.CandidateCount);

    //    var paymentStartDate = settlement.PersianExecutionDate;

    //    var paymentDeadlineDate = AddDays(
    //        paymentStartDate,
    //        4);

    //    var paymentStatus = CalculateStatus(
    //        paymentStartDate,
    //        paymentDeadlineDate);

    //    return new ExternalSettlementStatusDto
    //    {
    //        AgencyId = settlement.AgencyId,
    //        YearId = settlement.YearId,
    //        SettlmentId = settlement.Id,

    //        PersianExecutionDate = paymentStartDate,
    //        PaymentDeadline = paymentDeadlineDate,
    //        PaymentStatus = paymentStatus,

    //        // DebtAmount = settlement.TotalDebit,
    //        Balance = settlement.Balance,

    //        Items = items,

    //        TotalInPersonCount = totalInPersonCount,

    //        TotalOnlineCount = totalOnlineCount
    //    };
    //}


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

    private static SettlementPaymentStatus CalculateStatus(
        string paymentStartDate,
        string paymentDeadlineDate)
    {
        var today = DateTime.Today;

        var startDate = ParsePersianDate(paymentStartDate);
        var deadlineDate = ParsePersianDate(paymentDeadlineDate);

        if (today < startDate)
            return SettlementPaymentStatus.NotReached;

        if (today <= deadlineDate)
            return SettlementPaymentStatus.InPaymentPeriod;

        return SettlementPaymentStatus.Expired;
    }

    private static string AddDays(
        string persianDate,
        int days)
    {
        var date = ParsePersianDate(persianDate);

        var result = date.AddDays(days);

        var calendar = new PersianCalendar();

        var year = calendar.GetYear(result);
        var month = calendar.GetMonth(result);
        var day = calendar.GetDayOfMonth(result);

        return $"{year - 1400:00}/{month:00}/{day:00}";
    }

    private static DateTime ParsePersianDate(
        string value)
    {
        var parts = value.Split('/');

        if (parts.Length != 3)
            throw new FormatException(
                $"فرمت تاریخ نامعتبر است: {value}");

        var year = 1400 + int.Parse(parts[0]);
        var month = int.Parse(parts[1]);
        var day = int.Parse(parts[2]);

        var calendar = new PersianCalendar();

        return calendar.ToDateTime(
            year,
            month,
            day,
            0,
            0,
            0,
            0);
    }


    public async Task<List<string>> GetPersianAgenciesnDatesAsync(
    int agencyId,
    int yearId,
    CancellationToken cancellationToken)
    {
        return await _context.Settlements
            .AsNoTracking()
            .Where(x =>
                x.AgencyId == agencyId &&
                x.YearId == yearId &&
                !string.IsNullOrWhiteSpace(x.PersianExecutionDate))
            .Select(x => x.PersianExecutionDate)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }


    public async Task<SettlementOrderReportDto?> GetSettlementOrderReportAsync(
    int agencyId,
    int yearId,
    string persianExecutionDate,
    int registrationOrder,
    CancellationToken cancellationToken)
    {
        var currentOrder = await _context.SettlementOrders
            .AsNoTracking()
            .Where(x =>
                x.AgencyId == agencyId &&
                x.YearId == yearId &&
                x.RegistrationOrder == registrationOrder &&
                x.PersianExecutionDate == persianExecutionDate)
            .OrderByDescending(x => x.Id)
            .Select(x => new
            {
                x.Id,
                x.AgencyId,
                x.YearId,
                x.RegistrationOrder,
                x.PersianExecutionDate,
                x.Balance
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (currentOrder == null)
            return null;

        var previousExecutionDate = await _context.SettlementOrders
            .AsNoTracking()
            .Where(x =>
                x.AgencyId == agencyId &&
                x.YearId == yearId &&
                x.RegistrationOrder == registrationOrder &&
                string.Compare(x.PersianExecutionDate, persianExecutionDate) < 0)
            .OrderByDescending(x => x.PersianExecutionDate)
            .Select(x => x.PersianExecutionDate)
            .FirstOrDefaultAsync(cancellationToken);

        var items = await _context.SettlementOrderItems
            .AsNoTracking()
            .Where(x =>
                x.SettlementOrderId == currentOrder.Id &&
                x.ExamModeId == 1)
            .Select(x => new
            {
                x.EducationalLevelId,
                x.StudyFieldId,
                x.ExamModeId,
                x.CandidateCount
            })
            .ToListAsync(cancellationToken);

        var reportItems = items
            .GroupBy(x => new
            {
                x.EducationalLevelId,
                x.StudyFieldId
            })
            .Select(g => new SettlementOrderReportItemDto
            {
                EducationalLevelId = g.Key.EducationalLevelId,
                StudyFieldId = g.Key.StudyFieldId,
                ExamModeId = 1,
                CandidateCount = g.Sum(x => x.CandidateCount),
                Amount = 0
            })
            .OrderBy(x => x.EducationalLevelId)
            .ThenBy(x => x.StudyFieldId)
            .ToList();

        return new SettlementOrderReportDto
        {
            AgencyId = agencyId,
            YearId = yearId,
            RegistrationOrder = registrationOrder,
            PreviousExecutionDate = previousExecutionDate ?? string.Empty,
            PersianExecutionDate = persianExecutionDate,
            TotalCandidateCount = reportItems.Sum(x => x.CandidateCount),
            TotalAmount = currentOrder.Balance,
            Items = reportItems
        };
    }

}