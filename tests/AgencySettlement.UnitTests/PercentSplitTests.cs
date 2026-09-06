using Xunit;
using AgencySettlement.Domain.ValueObjects;

namespace AgencySettlement.UnitTests;

public sealed class PercentSplitTests
{
    [Theory]
    [InlineData(0, 100)]
    [InlineData(25, 75)]
    [InlineData(70, 30)]
    [InlineData(100, 0)]
    public void FromAgencyPercent_ShouldProduceComplement(decimal agency, decimal expectedGaj)
    {
        var split = PercentSplit.FromAgencyPercent(agency);
        Assert.Equal(100m, split.AgencyPercent + split.GajPercent);
        Assert.Equal(expectedGaj, split.GajPercent);
    }

    [Fact]
    public void Create_ShouldRejectNonHundredSplit()
    {
        Assert.Throws<ArgumentException>(() => PercentSplit.Create(25m, 70m));
    }
}
