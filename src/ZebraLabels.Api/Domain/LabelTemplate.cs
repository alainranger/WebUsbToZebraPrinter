namespace ZebraLabels.Api.Domain;

public sealed class LabelTemplate
{
    public LabelTemplate()
    {
    }

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public PrintLanguage SourceLanguage { get; set; }

    public string RawContent { get; set; } = string.Empty;

    public LabelDimensions Dimensions { get; set; } = new(102, 152, 8);

    public List<TemplateVariable> Variables { get; set; } = [];

    public bool IsArchived { get; set; }

    public int Version { get; set; } = 1;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}
