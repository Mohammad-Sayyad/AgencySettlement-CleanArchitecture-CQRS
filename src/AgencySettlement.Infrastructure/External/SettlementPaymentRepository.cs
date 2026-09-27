using AgencySettlement.Application.Abstractions.External;
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

  
public async Task AddSettlementPaymentAsync(
    SettlementPayment payment,
    CancellationToken cancellationToken)
        {
            var settlement = await _context.Settlements
                .FirstOrDefaultAsync(
                    x => x.Id == payment.SettlementId,
                    cancellationToken);

            if (settlement is null)
                throw new InvalidOperationException(
                    $"Settlement با شناسه {payment.SettlementId} پیدا نشد.");

            if (payment.IsAppliedToSettlement)
                return;

            await _context.SettlementPayments.AddAsync(
                payment,
                cancellationToken);

            settlement.TotalCredit += payment.Amount;

            settlement.Balance =
                settlement.TotalDebit -
                settlement.TotalCredit;

            payment.IsAppliedToSettlement = true;

            await _context.SaveChangesAsync(cancellationToken);
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


    }



}