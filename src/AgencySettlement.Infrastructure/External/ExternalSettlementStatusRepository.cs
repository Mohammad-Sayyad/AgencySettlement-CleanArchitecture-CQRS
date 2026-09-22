using AgencySettlement.Application.Abstractions.External;
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

        Console.WriteLine($"SERVER: [{serverName}]");
        Console.WriteLine($"DATABASE: [{databaseName}]");
        Console.WriteLine($"AgencyId: [{agencyId}]");
        Console.WriteLine($"YearId: [{yearId}]");
        Console.WriteLine($"PersianExecutionDate: [{persianExecutionDate}]");
        Console.WriteLine($"DateLength: {persianExecutionDate?.Length}");


        if (settlement == null)
            return null;

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
            Balance = settlement.Balance
        };
    }
    //
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