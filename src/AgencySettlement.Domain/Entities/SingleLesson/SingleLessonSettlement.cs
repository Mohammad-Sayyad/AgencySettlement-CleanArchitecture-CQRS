namespace AgencySettlement.Domain.Entities.SingleLesson;

public sealed class SingleLessonSettlement
{
    public long Id { get; set; }

    public int AgencyId { get; set; }

    public int YearId { get; set; }

    public string PersianExecutionDate { get; set; } = string.Empty;

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public decimal Balance { get; set; }

    public decimal TotalDebitGaj { get; set; }

    public decimal TotalCreditGaj { get; set; }

    public decimal BalanceGaj { get; set; }

    public decimal TotalAgencyAmount { get; set; }

    public decimal TotalGajAmount { get; set; }

    public int TotalCourseCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<SingleLessonSettlementItem> Items { get; set; }
        = new List<SingleLessonSettlementItem>();
}