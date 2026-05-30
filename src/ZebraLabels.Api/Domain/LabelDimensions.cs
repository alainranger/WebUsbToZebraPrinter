namespace ZebraLabels.Api.Domain;

public sealed class LabelDimensions
{
    private static readonly HashSet<int> SupportedDpmm = [6, 8, 12, 24];

    private LabelDimensions()
    {
    }

    public LabelDimensions(double widthMm, double heightMm, int dpmm)
    {
        if (widthMm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(widthMm), "Width must be positive.");
        }

        if (heightMm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightMm), "Height must be positive.");
        }

        if (!SupportedDpmm.Contains(dpmm))
        {
            throw new ArgumentOutOfRangeException(nameof(dpmm), "Dpmm must be one of: 6, 8, 12, 24.");
        }

        WidthMm = widthMm;
        HeightMm = heightMm;
        Dpmm = dpmm;
    }

    public double WidthMm { get; private set; }

    public double HeightMm { get; private set; }

    public int Dpmm { get; private set; }
}
