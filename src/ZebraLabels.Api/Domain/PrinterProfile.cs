namespace ZebraLabels.Api.Domain;

public sealed class PrinterProfile
{
    public PrinterProfile()
    {
    }

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? VendorId { get; set; }

    public int? ProductId { get; set; }

    public LanguageCapabilities Capabilities { get; set; } = new(true, true, false);

    public PrintLanguage PreferredLanguage { get; set; }

    public LabelDimensions DefaultDimensions { get; set; } = new(102, 152, 8);

    public string? Notes { get; set; }
}
