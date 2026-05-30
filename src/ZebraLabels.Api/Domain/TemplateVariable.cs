namespace ZebraLabels.Api.Domain;

public sealed class TemplateVariable
{
    private TemplateVariable()
    {
    }

    public TemplateVariable(string name, string? displayName, bool isRequired, string? defaultValue, string? exampleValue, int order)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Variable name is required.", nameof(name)) : name.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        IsRequired = isRequired;
        DefaultValue = string.IsNullOrWhiteSpace(defaultValue) ? null : defaultValue;
        ExampleValue = string.IsNullOrWhiteSpace(exampleValue) ? null : exampleValue;
        Order = order;
    }

    public string Name { get; private set; } = string.Empty;

    public string? DisplayName { get; private set; }

    public bool IsRequired { get; private set; }

    public string? DefaultValue { get; private set; }

    public string? ExampleValue { get; private set; }

    public int Order { get; private set; }
}
