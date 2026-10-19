
using AgencySettlement.Application.Abstractions.External.SingleLesson;
using AgencySettlement.Domain.Entities.SingleLesson;
using AgencySettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgencySettlement.Infrastructure.External.SingleLesson;

public sealed class SingleLessonSettlementStore
    : ISingleLessonSettlementStore
{
    private readonly AgencySettlementDbContext _db;

    public SingleLessonSettlementStore(
        AgencySettlementDbContext db)
    {
        _db = db;
    }

    public async Task<List<SingleLessonExternalInput>> GetInputsAsync(
        int agencyId,
        int yearId,
        string date,
        CancellationToken ct)
    {
        return await _db.SingleLessonExternalInputs
            .AsNoTracking()
            .Where(x =>
                x.AgencyId == agencyId &&
                x.YearId == yearId &&
                x.PersianExecutionDate == date)
            .ToListAsync(ct);
    }

    public async Task<List<SingleLessonExternalInput>> GetInputsAsync(
        IReadOnlyCollection<(
            int AgencyId,
            int YearId,
            string PersianExecutionDate)> groups,
        CancellationToken ct)
    {
        if (groups.Count == 0)
            return [];

        var keys = groups.ToHashSet();

        var agencyIds = groups
            .Select(x => x.AgencyId)
            .Distinct()
            .ToArray();

        var yearIds = groups
            .Select(x => x.YearId)
            .Distinct()
            .ToArray();

        var dates = groups
            .Select(x => x.PersianExecutionDate)
            .Distinct()
            .ToArray();

        var candidates = await _db.SingleLessonExternalInputs
            .AsNoTracking()
            .Where(x =>
                agencyIds.Contains(x.AgencyId) &&
                yearIds.Contains(x.YearId) &&
                dates.Contains(x.PersianExecutionDate))
            .ToListAsync(ct);

        return candidates
            .Where(x => keys.Contains((
                x.AgencyId,
                x.YearId,
                x.PersianExecutionDate)))
            .ToList();
    }

    public async Task<SingleLessonPrice?> GetPriceAsync(
        SingleLessonExternalInput input,
        CancellationToken ct)
    {
        return await _db.SingleLessonPrices
            .AsNoTracking()
            .Where(p =>
                p.IsActive &&
                p.YearId == input.YearId &&
                p.CourseId == input.CourseId &&
                p.StudyFieldId == input.StudyFieldId &&
                p.EducationalLevelId == input.EducationalLevelId &&
                p.PackageId == input.PackageId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<SingleLessonPercent?> GetPercentAsync(
        SingleLessonExternalInput input,
        CancellationToken ct)
    {
        return await _db.SingleLessonPercents
            .AsNoTracking()
            .Where(p =>
                p.IsActive &&
                p.YearId == input.YearId &&
                p.CourseId == input.CourseId &&
                p.StudyFieldId == input.StudyFieldId &&
                p.EducationalLevelId == input.EducationalLevelId &&
                p.PackageId == input.PackageId &&
                (p.AgencyId == input.AgencyId || p.AgencyId == null))
            .OrderByDescending(p => p.AgencyId == input.AgencyId)
            .ThenByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<SingleLessonPrice>> GetPricesAsync(
        IReadOnlyCollection<SingleLessonExternalInput> inputs,
        CancellationToken ct)
    {
        if (inputs.Count == 0)
            return [];

        var years = inputs
            .Select(x => x.YearId)
            .Distinct()
            .ToArray();

        var courses = inputs
            .Select(x => x.CourseId)
            .Distinct()
            .ToArray();

        var fields = inputs
            .Select(x => x.StudyFieldId)
            .Distinct()
            .ToArray();

        var levels = inputs
            .Select(x => x.EducationalLevelId)
            .Distinct()
            .ToArray();

        var packages = inputs
            .Select(x => x.PackageId)
            .Distinct()
            .ToArray();

        return await _db.SingleLessonPrices
            .AsNoTracking()
            .Where(p =>
                p.IsActive &&
                years.Contains(p.YearId) &&
                courses.Contains(p.CourseId) &&
                fields.Contains(p.StudyFieldId) &&
                levels.Contains(p.EducationalLevelId) &&
                packages.Contains(p.PackageId))
            .ToListAsync(ct);
    }

    public async Task<List<SingleLessonPercent>> GetPercentsAsync(
        IReadOnlyCollection<SingleLessonExternalInput> inputs,
        CancellationToken ct)
    {
        if (inputs.Count == 0)
            return [];

        var years = inputs
            .Select(x => x.YearId)
            .Distinct()
            .ToArray();

        var courses = inputs
            .Select(x => x.CourseId)
            .Distinct()
            .ToArray();

        var fields = inputs
            .Select(x => x.StudyFieldId)
            .Distinct()
            .ToArray();

        var levels = inputs
            .Select(x => x.EducationalLevelId)
            .Distinct()
            .ToArray();

        var packages = inputs
            .Select(x => x.PackageId)
            .Distinct()
            .ToArray();

        var agencies = inputs
            .Select(x => x.AgencyId)
            .Distinct()
            .ToArray();

        return await _db.SingleLessonPercents
            .AsNoTracking()
            .Where(p =>
                p.IsActive &&
                years.Contains(p.YearId) &&
                courses.Contains(p.CourseId) &&
                fields.Contains(p.StudyFieldId) &&
                levels.Contains(p.EducationalLevelId) &&
                packages.Contains(p.PackageId) &&
                (p.AgencyId == null ||
                 agencies.Contains(p.AgencyId.Value)))
            .ToListAsync(ct);
    }

    public async Task<decimal> GetAppliedSingleLessonPaymentsAsync(
        long settlementId,
        CancellationToken ct)
    {
        return await _db.SettlementPayments
            .AsNoTracking()
            .Where(p =>
                p.IsAppliedToSettlement &&
                p.SingleLessonSettlementId == settlementId)
            .SumAsync(
                p => (decimal?)p.SingleLessonAmount,
                ct) ?? 0m;
    }

    public async Task<SingleLessonSettlement?> GetSettlementAsync(
        int agencyId,
        int yearId,
        string date,
        CancellationToken ct)
    {
        return await _db.SingleLessonSettlements
            .FirstOrDefaultAsync(x =>
                x.AgencyId == agencyId &&
                x.YearId == yearId &&
                x.PersianExecutionDate == date,
                ct);
    }

    public async Task ReplaceItemsAsync(
        SingleLessonSettlement settlement,
        IReadOnlyCollection<SingleLessonSettlementItem> items,
        CancellationToken ct)
    {
        await _db.SingleLessonSettlementItems
            .Where(x => x.SettlementId == settlement.Id)
            .ExecuteDeleteAsync(ct);

        if (items.Count == 0)
            return;

        foreach (var item in items)
            item.SettlementId = settlement.Id;

        await _db.SingleLessonSettlementItems
            .AddRangeAsync(items, ct);
    }

    public async Task AddSettlementAsync(
        SingleLessonSettlement settlement,
        CancellationToken ct)
    {
        await _db.SingleLessonSettlements
            .AddAsync(settlement, ct);
    }

    public async Task AddHistoryAsync(
        SingleLessonSettlementHistory history,
        CancellationToken ct)
    {
        await _db.SingleLessonSettlementHistories
            .AddAsync(history, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
