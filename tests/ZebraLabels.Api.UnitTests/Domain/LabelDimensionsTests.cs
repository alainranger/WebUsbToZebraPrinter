using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.UnitTests.Domain;

public sealed class LabelDimensionsTests
{
    [Fact]
    public void Constructor_CreatesDimensions_WhenValuesAreSupported()
    {
        var dimensions = new LabelDimensions(102, 152, 8);

        Assert.Equal(102, dimensions.WidthMm);
        Assert.Equal(152, dimensions.HeightMm);
        Assert.Equal(8, dimensions.Dpmm);
    }

    [Theory]
    [InlineData(0, 152, 8)]
    [InlineData(102, 0, 8)]
    [InlineData(102, 152, 5)]
    public void Constructor_Throws_WhenValuesAreInvalid(double widthMm, double heightMm, int dpmm)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LabelDimensions(widthMm, heightMm, dpmm));
    }
}
