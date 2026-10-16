using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Domain.Entities.SingleLesson;
using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.External.SingleLesson;

public sealed class SingleLessonSettlementStore : ISingleLessonSettlementStore
{
    private readonly AgencySettlementDbContext _db;
    public SingleLessonSettlementStore(AgencySettlementDbContext db) => _db = db;

    public Task<List<SingleLessonExternalInput>> GetInputsAsync(int agencyId, int yearId, string date, CancellationToken ct) =>
        _db.SingleLessonExternalInputs.Where(x => x.AgencyId == agencyId && x.YearId == yearId && x.PersianExecutionDate == date).ToListAsync(ct);

    public Task<SingleLessonPrice?> GetPriceAsync(SingleLessonExternalInput x, CancellationToken ct) =>
        _db.Set<SingleLessonPrice>().Where(p => p.IsActive && p.YearId == x.YearId && p.CourseId == x.CourseId && p.StudyFieldId == x.StudyFieldId && p.EducationalLevelId == x.EducationalLevelId && p.PackageId == x.PackageId)
          .OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id).FirstOrDefaultAsync(ct);

    public Task<SingleLessonPercent?> GetPercentAsync(SingleLessonExternalInput x, CancellationToken ct) =>
        _db.Set<SingleLessonPercent>().Where(p => p.IsActive && p.YearId == x.YearId && p.CourseId == x.CourseId && p.StudyFieldId == x.StudyFieldId && p.EducationalLevelId == x.EducationalLevelId && p.PackageId == x.PackageId && (p.AgencyId == x.AgencyId || p.AgencyId == null))
          .OrderByDescending(p => p.AgencyId == x.AgencyId).ThenByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id).FirstOrDefaultAsync(ct);

    public Task<SingleLessonSettlement?> GetSettlementAsync(int agencyId, int yearId, string date, CancellationToken ct) =>
        _db.Set<SingleLessonSettlement>().Include(x => x.Items).FirstOrDefaultAsync(x => x.AgencyId == agencyId && x.YearId == yearId && x.PersianExecutionDate == date, ct);

    public async Task ReplaceItemsAsync(SingleLessonSettlement settlement, IReadOnlyCollection<SingleLessonSettlementItem> items, CancellationToken ct)
    {
        var existing = await _db.Set<SingleLessonSettlementItem>().Where(x => x.SettlementId == settlement.Id).ToListAsync(ct);
        _db.RemoveRange(existing);
        foreach (var item in items) { item.SettlementId = settlement.Id; _db.Add(item); }
    }

    public Task AddSettlementAsync(SingleLessonSettlement settlement, CancellationToken ct) => _db.Set<SingleLessonSettlement>().AddAsync(settlement, ct).AsTask();
    public Task AddHistoryAsync(SingleLessonSettlementHistory history, CancellationToken ct) => _db.Set<SingleLessonSettlementHistory>().AddAsync(history, ct).AsTask();
    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
