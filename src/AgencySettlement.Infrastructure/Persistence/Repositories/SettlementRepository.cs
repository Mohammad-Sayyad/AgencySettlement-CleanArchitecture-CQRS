using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    //public sealed class SettlementRepository
    //: ISettlementRepository
    //{
    //    private readonly AgencySettlementDbContext _db;

    //    public SettlementRepository(
    //        AgencySettlementDbContext db)
    //    {
    //        _db = db;
    //    }

    //    public async Task AddAsync(
    //        Settlement settlement,
    //        CancellationToken cancellationToken)
    //    {
    //        await _db.Settlements.AddAsync(
    //            settlement,
    //            cancellationToken);
    //    }

    //    public async Task SaveChangesAsync(
    //        CancellationToken cancellationToken)
    //    {
    //        await _db.SaveChangesAsync(
    //            cancellationToken);
    //    }



    //    public async Task<List<SettlementItem>> GetSettlementItemsAsync(
    //   long settlementId,
    //   CancellationToken cancellationToken)
    //    {
    //        return await _db.SettlementItems
    //            .AsNoTracking()
    //            .Where(x => x.SettlementId == settlementId)
    //            .OrderBy(x => x.EducationalLevelId)
    //            .ThenBy(x => x.StudyFieldId)
    //            .ToListAsync(cancellationToken);
    //    }
    //}


    public sealed class SettlementRepository
    : ISettlementRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementRepository(AgencySettlementDbContext context)
        {
            _context = context;
        }

        public Task<Settlement?> GetByAgencyAndYearAsync(
            int agencyId,
            int yearId,
            CancellationToken cancellationToken)
        {
            return _context.Settlements
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(
                    x =>
                        x.AgencyId == agencyId &&
                        x.YearId == yearId,
                    cancellationToken);
        }

        public async Task AddAsync(
            Settlement entity,
            CancellationToken cancellationToken)
        {
            await _context.Settlements.AddAsync(
                entity,
                cancellationToken);
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
