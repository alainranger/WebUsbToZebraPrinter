using ZebraLabels.Api.Domain;
using ZebraLabels.Api.Infrastructure.Preview;

namespace ZebraLabels.Api.Features.Preview;

public static class PreviewEndpoints
{
    public static IEndpointRouteBuilder MapPreviewEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/preview").WithTags("Preview");

        group.MapPost("/render", async (RawRenderRequest request, PreviewRequestPolicy previewPolicy, LabelizeClient labelizeClient, CancellationToken cancellationToken) =>
        {
            Dictionary<string, string[]> errors;

            try
            {
                var dimensions = new LabelDimensions(request.Dimensions.WidthMm, request.Dimensions.HeightMm, request.Dimensions.Dpmm);
                errors = previewPolicy.Validate(request.Content, dimensions);

                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                var document = await labelizeClient.RenderAsync(request.Language, request.Content, dimensions, request.OutputType, cancellationToken);
                return Results.File(document.Content, document.ContentType);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                errors = new Dictionary<string, string[]>
                {
                    [$"dimensions.{exception.ParamName}"] = [exception.Message]
                };

                return Results.ValidationProblem(errors);
            }
        });

        return endpoints;
    }

    public sealed record RawRenderRequest(
        PrintLanguage Language,
        string Content,
        LabelDimensionsDto Dimensions,
        PreviewOutputType OutputType);

    public sealed record LabelDimensionsDto(
        double WidthMm,
        double HeightMm,
        int Dpmm);
}
