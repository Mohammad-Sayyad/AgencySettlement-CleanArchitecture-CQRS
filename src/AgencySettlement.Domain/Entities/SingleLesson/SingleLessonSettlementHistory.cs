namespace AgencySettlement.Domain.Entities.SingleLesson;

public sealed class SingleLessonSettlementHistory
{
    public long Id { get; set; }
    public long SettlementId { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal TotalDebitGaj { get; set; }
    public decimal TotalCreditGaj { get; set; }
    public decimal Balance { get; set; }
    public decimal TotalAgencyAmount { get; set; }
    public decimal TotalGajAmount { get; set; }
    public decimal BalanceGaj { get; set; }
    public int TotalCourseCount { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
