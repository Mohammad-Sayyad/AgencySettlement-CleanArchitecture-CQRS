using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.DTOs.ReportsDto;
using AgencySettlement.Domain.Entities;
using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.External
{
    public sealed class SettlementPaymentRepository
        : ISettlementPaymentRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementPaymentRepository(
            AgencySettlementDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetPaymentAmountAsync(
            int agencyId,
            int yearId,
            string persianExecutionDate,
            CancellationToken cancellationToken)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(
                    x =>
                        x.AgencyId == agencyId &&
                        x.YearId == yearId &&
                        x.PersianExecutionDate ==
                            persianExecutionDate)
                .SumAsync(
                    x => x.Amount,
                    cancellationToken);
        }

        //public async Task AddSettlementPaymentAsync(
        //    SettlementPayment payment,
        //    CancellationToken cancellationToken)

        //{




        //    await _context.SettlementPayments.AddAsync(
        //        payment,
        //        cancellationToken);

        //    await _context.SaveChangesAsync(
        //        cancellationToken);
        //}


        public async Task<long> AddSettlementPaymentAsync(
                SettlementPayment payment,
                CancellationToken cancellationToken)
        {
            if (payment.ExamAmount < 0 || payment.SingleLessonAmount < 0)
                throw new ArgumentException("مبلغ پرداخت نمی‌تواند منفی باشد.");

            if (payment.ExamAmount + payment.SingleLessonAmount <= 0)
                throw new ArgumentException("مبلغ پرداخت باید بیشتر از صفر باشد.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            // بررسی شماره پیگیری تکراری
            var duplicateExists = await _context.SettlementPayments
                .AnyAsync(
                    x => x.AgencyId == payment.AgencyId
                         && x.YearId == payment.YearId
                         && x.TrackingNumber == payment.TrackingNumber,
                    cancellationToken);

            if (duplicateExists)
                throw new InvalidOperationException(
                    "این شماره پیگیری قبلاً برای این نمایندگی و سال ثبت شده است.");

            // دریافت تسویه آزمون با قفل تراکنشی در زمان به‌روزرسانی
            var examSettlement = await _context.Settlements
                .SingleOrDefaultAsync(
                    x => x.Id == payment.SettlementId
                         && x.AgencyId == payment.AgencyId
                         && x.YearId == payment.YearId
                         && x.PersianExecutionDate == payment.PersianExecutionDate,
                    cancellationToken);

            if (examSettlement is null)
                throw new KeyNotFoundException(
                    "تسویه آزمون با اطلاعات ارسالی پیدا نشد.");

            // تسویه تک‌درس تنها در صورت وجود مبلغ تک‌درس الزامی است
            Domain.Entities.SingleLesson.SingleLessonSettlement? singleLessonSettlement = null;

            if (payment.SingleLessonAmount > 0)
            {
                singleLessonSettlement = await _context.SingleLessonSettlements
                    .SingleOrDefaultAsync(
                        x => x.AgencyId == payment.AgencyId
                             && x.YearId == payment.YearId
                             && x.PersianExecutionDate == payment.PersianExecutionDate,
                        cancellationToken);

                if (singleLessonSettlement is null)
                    throw new KeyNotFoundException(
                        "تسویه تک‌درس برای نمایندگی، سال و تاریخ موردنظر پیدا نشد.");

                payment.SingleLessonSettlementId = singleLessonSettlement.Id;
            }
            else
            {
                payment.SingleLessonSettlementId = null;
            }

            // محاسبه مبلغ کل در سمت سرور
            payment.Amount = payment.ExamAmount + payment.SingleLessonAmount;

            // افزایش بستانکاری آزمون فقط به اندازه مبلغ پرداختی بابت آزمون
            examSettlement.TotalCredit += payment.ExamAmount;
            examSettlement.Balance =
                examSettlement.TotalDebit - examSettlement.TotalCredit;

            // افزایش بستانکاری تک‌درس فقط به اندازه مبلغ پرداختی بابت تک‌درس
            if (singleLessonSettlement is not null)
            {
                singleLessonSettlement.TotalCredit += payment.SingleLessonAmount;
                singleLessonSettlement.Balance =
                    singleLessonSettlement.TotalDebit -
                    singleLessonSettlement.TotalCredit;

                singleLessonSettlement.UpdatedAt = DateTime.UtcNow;
            }

            payment.IsAppliedToSettlement = true;

            _context.SettlementPayments.Add(payment);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return payment.Id;
        }



        public async Task<Settlement?> GetSettlementByIdAsync(
            long settlementId,
            CancellationToken cancellationToken)
        {
            return await _context.Settlements
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == settlementId,
                    cancellationToken);
        }

        public IQueryable<SettlementPayment> GetSettlementPayments()
        {
            return _context.SettlementPayments
                .AsNoTracking();
        }

        public async Task<Settlement?> GetSettlementByAgencyAndDateAsync(
    int agencyId,
    int yearId,
    string persianExecutionDate,
    CancellationToken cancellationToken)
        {
            return await _context.Settlements
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.AgencyId == agencyId &&
                        x.YearId == yearId &&
                        x.PersianExecutionDate == persianExecutionDate,
                    cancellationToken);
        }


        public async Task<List<SettlementPaymentReportDto>> GetReportAsync(
        int? yearId,
        string? persianExecutionDate,
        int? agencyId,
        CancellationToken cancellationToken)
        {
            var query =
                from payment in _context.SettlementPayments.AsNoTracking()
                join agency in _context.Agencies.AsNoTracking()
                    on payment.AgencyId equals agency.Id
                select new { payment, agency };

            if (yearId.HasValue)
            {
                query = query.Where(
                    x => x.payment.YearId == yearId.Value);
            }

            if (!string.IsNullOrWhiteSpace(persianExecutionDate))
            {
                query = query.Where(
                    x => x.payment.PersianExecutionDate
                        == persianExecutionDate);
            }

            if (agencyId.HasValue)
            {
                query = query.Where(
                    x => x.payment.AgencyId == agencyId.Value);
            }

            return await query
                .OrderByDescending(x => x.payment.YearId)
                .ThenByDescending(x => x.payment.PaymentDate)
                .ThenByDescending(x => x.payment.Id)
                .Select(x => new SettlementPaymentReportDto
                {
                    PaymentId = x.payment.Id,
                    AgencyId = x.payment.AgencyId,
                    AgencyName = x.agency.Name,
                    DetailCode = x.agency.DetailCode,

                    YearId = x.payment.YearId,
                    PersianExecutionDate = x.payment.PersianExecutionDate,
                    PaymentDate = x.payment.PaymentDate,
                    TrackingNumber = x.payment.TrackingNumber,

                    ExamAmount = x.payment.ExamAmount,
                    SingleLessonAmount = x.payment.SingleLessonAmount,

                    TotalPaymentAmount =
                        x.payment.ExamAmount
                        + x.payment.SingleLessonAmount,

                    Amount = x.payment.Amount,
                    IsAppliedToSettlement = x.payment.IsAppliedToSettlement
                })
                .ToListAsync(cancellationToken);
        }

    }



}