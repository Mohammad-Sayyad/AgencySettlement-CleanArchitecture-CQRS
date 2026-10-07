using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.ExternalDtos;
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

        var databaseName = _context.Database.GetDbConnection().Database;
        var serverName = _context.Database.GetDbConnection().DataSource;

        if (settlement == null)
            return null;

        // -----------------------------------------
        // Items
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

        var totalInPersonCount = items
            .Where(x => x.ExamModeId == 0)
            .Sum(x => x.CandidateCount);

        var totalOnlineCount = items
            .Where(x => x.ExamModeId == 1)
            .Sum(x => x.CandidateCount);

        var paymentStartDate = settlement.PersianExecutionDate;

        var paymentDeadlineDate = AddDays(
            paymentStartDate,
            4);

        var paymentStatus = CalculateStatus(
            paymentStartDate,
            paymentDeadlineDate);

        return new ExternalSettlementStatusDto
        {
            AgencyId = settlement.AgencyId,
            YearId = settlement.YearId,
            SettlmentId = settlement.Id,

            PersianExecutionDate = paymentStartDate,
            PaymentDeadline = paymentDeadlineDate,
            PaymentStatus = paymentStatus,

            // DebtAmount = settlement.TotalDebit,
            Balance = settlement.Balance,

            Items = items,

            TotalInPersonCount = totalInPersonCount,

            TotalOnlineCount = totalOnlineCount
        };
    }

    //public async Task<ExternalSettlementStatusDto?> GetExternalSettlementStatusAsync(
    //    int agencyId,
    //    int yearId,
    //    string persianExecutionDate,
    //    CancellationToken cancellationToken)
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
    //       // DebtAmount = settlement.TotalDebit,
    //        Balance = settlement.Balance
    //    };
    //}
    //

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