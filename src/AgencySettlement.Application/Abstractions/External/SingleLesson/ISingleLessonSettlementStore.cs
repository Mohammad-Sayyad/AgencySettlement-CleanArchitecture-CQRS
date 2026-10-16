using AgencySettlement.Domain.Entities.SingleLesson;

namespace AgencySettlement.Application.Abstractions.External.SingleLesson;

public interface ISingleLessonSettlementStore
{
    Task<List<SingleLessonExternalInput>> GetInputsAsync(int agencyId, int yearId, string date, CancellationToken ct);
    Task<SingleLessonPrice?> GetPriceAsync(SingleLessonExternalInput input, CancellationToken ct);
    Task<SingleLessonPercent?> GetPercentAsync(SingleLessonExternalInput input, CancellationToken ct);
    Task<SingleLessonSettlement?> GetSettlementAsync(int agencyId, int yearId, string date, CancellationToken ct);
    Task ReplaceItemsAsync(SingleLessonSettlement settlement, IReadOnlyCollection<SingleLessonSettlementItem> items, CancellationToken ct);
    Task AddSettlementAsync(SingleLessonSettlement settlement, CancellationToken ct);
    Task AddHistoryAsync(SingleLessonSettlementHistory history, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
