public class Percent
{
    public long Id { get; set; }

    public int AgencyId { get; set; }

    public int ExamModeId { get; set; }

    public string PersianExecutionDate { get; set; }
    public decimal AgencyPercent { get; set; }

    public decimal GajPercent { get; set; }

    public decimal StudentPercent { get; set; }

    public int YearId { get; set; }
    public bool IsActive { get; set; }
}