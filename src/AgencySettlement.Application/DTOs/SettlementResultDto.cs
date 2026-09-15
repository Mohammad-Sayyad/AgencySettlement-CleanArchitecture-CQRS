using AgencySettlement.Application.DTOs;

public sealed class SettlementResultDto
{
    public long SettlementId { get; set; }

    public int AgencyId { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public decimal Balance { get; set; }

    public List<SettlementItemResultDto> Items { get; set; } = [];

    public int FreeQuotaCount { get; set; }

    public int OneHundredThousandQuotaCount { get; set; }

    public decimal ContractFloorAmount { get; set; }

    public decimal TotalDebitGaj { get; set; }

    public decimal TotalCreditGaj { get; set; }
    public decimal BalanceGaj { get; set; }
    public string PersianExecutionDate { get; set; } = string.Empty;
}