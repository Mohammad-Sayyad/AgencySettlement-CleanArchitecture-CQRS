namespace AgencySettlement.Domain.ValueObjects;

public readonly record struct PercentSplit
{
    public decimal AgencyPercent { get; }
    public decimal GajPercent { get; }

    private PercentSplit(decimal agencyPercent)
    {
        AgencyPercent = agencyPercent;
        GajPercent = 100m - agencyPercent;
    }

    public static PercentSplit Create(decimal agencyPercent, decimal gajPercent)
    {
        if (agencyPercent is < 0m or > 100m)
            throw new ArgumentOutOfRangeException(nameof(agencyPercent));

        if (gajPercent is < 0m or > 100m)
            throw new ArgumentOutOfRangeException(nameof(gajPercent));

        if (agencyPercent + gajPercent != 100m)
            throw new ArgumentException("AgencyPercent + GajPercent must equal exactly 100%.");

        return new PercentSplit(agencyPercent);
    }

    public static PercentSplit FromAgencyPercent(decimal agencyPercent)
    {
        if (agencyPercent is < 0m or > 100m)
            throw new ArgumentOutOfRangeException(nameof(agencyPercent));

        return new PercentSplit(agencyPercent);
    }
}
