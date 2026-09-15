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
                x.YearId,
                x.PersianExecutionDate,
                x.TotalDebit
            })
            .FirstOrDefaultAsync(cancellationToken);

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
            DebtAmount = settlement.TotalDebit
        };
    }

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

}