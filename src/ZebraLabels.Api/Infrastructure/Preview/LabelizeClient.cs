using System.Net.Http.Headers;
using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.Infrastructure.Preview;

public sealed class LabelizeClient(HttpClient httpClient)
{
    public async Task<PreviewDocument> RenderAsync(
        PrintLanguage language,
        string content,
        LabelDimensions dimensions,
        PreviewOutputType outputType,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/convert?width={dimensions.WidthMm}&height={dimensions.HeightMm}&dpmm={dimensions.Dpmm}&output={outputType.ToString().ToLowerInvariant()}");

        request.Content = new StringContent(content);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(language == PrintLanguage.Epl ? "application/epl" : "application/zpl");

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType
            ?? (outputType == PreviewOutputType.Pdf ? "application/pdf" : "image/png");

        return new PreviewDocument(bytes, contentType);
    }
}
