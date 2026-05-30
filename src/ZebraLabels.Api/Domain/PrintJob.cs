namespace ZebraLabels.Api.Domain;

public sealed class PrintJob
{
    public PrintJob()
    {
    }

    public Guid Id { get; set; }

    public Guid? TemplateId { get; set; }

    public Guid PrinterProfileId { get; set; }

    public PrintLanguage RequestedLanguage { get; set; }

    public PrintLanguage EffectiveLanguage { get; set; }

    public PrintJobStatus Status { get; set; }

    public string? FailureReason { get; set; }

    public string Checksum { get; set; } = string.Empty;

    public DateTimeOffset SubmittedAtUtc { get; set; }

    public DateTimeOffset? SentAtUtc { get; set; }
}
