namespace ZebraLabels.Api.Infrastructure.Preview;

public sealed record PreviewDocument(
    byte[] Content,
    string ContentType);
