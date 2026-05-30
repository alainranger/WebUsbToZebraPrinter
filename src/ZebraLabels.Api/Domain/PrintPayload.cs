namespace ZebraLabels.Api.Domain;

public sealed record PrintPayload(
    string Content,
    string Checksum,
    PrintLanguage Language);
