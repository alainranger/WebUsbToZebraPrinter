namespace ZebraLabels.Api.Domain;

public enum PrintJobStatus
{
    Draft = 0,
    Submitted = 1,
    Rendered = 2,
    Sent = 3,
    Failed = 4
}
