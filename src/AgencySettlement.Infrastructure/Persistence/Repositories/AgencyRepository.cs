using AgencySettlement.Application.Abstractions.Persistence.Repositories;
using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.Persistence.Repositories;

public sealed class AgencyRepository : IAgencyRepository
{
    private readonly AgencySettlementDbContext _db;

    public AgencyRepository(
        AgencySettlementDbContext db)
    {
        _db = db;
    }

    public async Task<Agency?> GetByIdAsync(
        int agencyId,
        CancellationToken cancellationToken)
    {
        return await _db.Agencies
            .FirstOrDefaultAsync(
                x => x.Id == agencyId,
                cancellationToken);
    }

    public async Task<int> ConsumeFreeQuotaAsync(
        int agencyId,
        int requestedCount,
        CancellationToken cancellationToken)
    {
        if (requestedCount <= 0)
            return 0;

        var agency = await _db.Agencies
            .Where(x => x.Id == agencyId)
            .Select(x => new
            {
                x.Id,
                x.FreeQuotaCount
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (agency is null)
        {
            throw new InvalidOperationException(
                "نماینده پیدا نشد.");
        }

        var consumedCount = Math.Min(
            agency.FreeQuotaCount,
            requestedCount);

        if (consumedCount <= 0)
            return 0;

        var affectedRows = await _db.Agencies
            .Where(x =>
                x.Id == agencyId &&
                x.FreeQuotaCount >= consumedCount)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.FreeQuotaCount,
                    x => x.FreeQuotaCount - consumedCount),
                cancellationToken);

        if (affectedRows == 0)
        {
            throw new DbUpdateConcurrencyException(
                "مصرف سهمیه به دلیل تغییر همزمان سهمیه انجام نشد.");
        }

        return consumedCount;
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}