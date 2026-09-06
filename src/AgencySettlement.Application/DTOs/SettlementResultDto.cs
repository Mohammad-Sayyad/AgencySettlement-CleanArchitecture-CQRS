using AgencySettlement.Application.DTOs;

public sealed class SettlementResultDto
{
    public long SettlementId { get; set; }

    public int AgencyId { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public decimal Balance { get; set; }

    public List<SettlementItemResultDto> Items { get; set; } = [];
}