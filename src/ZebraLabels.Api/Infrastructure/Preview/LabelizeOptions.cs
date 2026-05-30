namespace ZebraLabels.Api.Infrastructure.Preview;

public sealed class LabelizeOptions
{
    public const string SectionName = "Labelize";

    public string BaseUrl { get; set; } = "http://labelize";

    public int MaxContentLength { get; set; } = 50_000;
}
