using Microsoft.EntityFrameworkCore;
using ZebraLabels.Api.Domain;
using ZebraLabels.Api.Infrastructure.Persistence;
using ZebraLabels.Api.Infrastructure.Preview;
using ZebraLabels.Api.Infrastructure.Rendering;

namespace ZebraLabels.Api.Features.Templates;

public static class TemplateEndpoints
{
    public static IEndpointRouteBuilder MapTemplateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/templates").WithTags("Templates");

        group.MapGet("/", async (ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
            TypedResults.Ok(await dbContext.Templates
                .AsNoTracking()
                .Where(template => !template.IsArchived)
                .OrderBy(template => template.Name)
                .Select(template => new LabelTemplateSummaryResponse(template.Id, template.Name, template.SourceLanguage, template.Version))
                .ToListAsync(cancellationToken)));

        group.MapPost("/", async (CreateOrUpdateTemplateRequest request, ZebraLabelsDbContext dbContext, TimeProvider timeProvider, CancellationToken cancellationToken) =>
        {
            var errors = TemplateRequestValidator.ValidateCreateOrUpdate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var variables = request.Variables ?? [];
            var now = timeProvider.GetUtcNow();
            var template = new LabelTemplate
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                SourceLanguage = request.SourceLanguage,
                RawContent = request.RawContent,
                Dimensions = new LabelDimensions(request.Dimensions!.WidthMm, request.Dimensions.HeightMm, request.Dimensions.Dpmm),
                Variables = variables
                    .OrderBy(variable => variable.Order)
                    .Select(variable => new TemplateVariable(variable.Name, variable.DisplayName, variable.IsRequired, variable.DefaultValue, variable.ExampleValue, variable.Order))
                    .ToList(),
                Version = 1,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            dbContext.Templates.Add(template);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/templates/{template.Id}", MapDetails(template));
        });

        group.MapGet("/{templateId:guid}", async (Guid templateId, ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var template = await dbContext.Templates
                .AsNoTracking()
                .Include(value => value.Variables)
                .SingleOrDefaultAsync(value => value.Id == templateId && !value.IsArchived, cancellationToken);

            return template is null ? Results.NotFound() : Results.Ok(MapDetails(template));
        });

        group.MapPut("/{templateId:guid}", async (Guid templateId, CreateOrUpdateTemplateRequest request, ZebraLabelsDbContext dbContext, TimeProvider timeProvider, CancellationToken cancellationToken) =>
        {
            var errors = TemplateRequestValidator.ValidateCreateOrUpdate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var template = await dbContext.Templates
                .Include(value => value.Variables)
                .SingleOrDefaultAsync(value => value.Id == templateId && !value.IsArchived, cancellationToken);

            if (template is null)
            {
                return Results.NotFound();
            }

            var variables = request.Variables ?? [];
            template.Name = request.Name.Trim();
            template.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            template.SourceLanguage = request.SourceLanguage;
            template.RawContent = request.RawContent;
            template.Dimensions = new LabelDimensions(request.Dimensions!.WidthMm, request.Dimensions.HeightMm, request.Dimensions.Dpmm);
            template.Variables = variables
                .OrderBy(variable => variable.Order)
                .Select(variable => new TemplateVariable(variable.Name, variable.DisplayName, variable.IsRequired, variable.DefaultValue, variable.ExampleValue, variable.Order))
                .ToList();
            template.Version += 1;
            template.UpdatedAtUtc = timeProvider.GetUtcNow();

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(MapDetails(template));
        });

        group.MapDelete("/{templateId:guid}", async (Guid templateId, ZebraLabelsDbContext dbContext, TimeProvider timeProvider, CancellationToken cancellationToken) =>
        {
            var template = await dbContext.Templates
                .SingleOrDefaultAsync(value => value.Id == templateId && !value.IsArchived, cancellationToken);

            if (template is null)
            {
                return Results.NotFound();
            }

            template.IsArchived = true;
            template.UpdatedAtUtc = timeProvider.GetUtcNow();
            template.Version += 1;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        group.MapPost("/{templateId:guid}/render", async (Guid templateId, RenderTemplateRequest request, ZebraLabelsDbContext dbContext, TemplateRenderingService renderingService, PreviewRequestPolicy previewPolicy, LabelizeClient labelizeClient, CancellationToken cancellationToken) =>
        {
            var template = await dbContext.Templates
                .AsNoTracking()
                .Include(value => value.Variables)
                .SingleOrDefaultAsync(value => value.Id == templateId && !value.IsArchived, cancellationToken);

            if (template is null)
            {
                return Results.NotFound();
            }

            var renderResult = renderingService.TryRender(template, request.VariableValues ?? new Dictionary<string, string?>());
            if (renderResult.Errors.Count > 0)
            {
                return Results.ValidationProblem(renderResult.Errors);
            }

            var payload = renderResult.Payload!;
            var previewErrors = previewPolicy.Validate(payload.Content, template.Dimensions);
            foreach (var error in TemplateRequestValidator.ValidateContentMatchesLanguage(payload.Language, payload.Content))
            {
                previewErrors[error.Key] = error.Value;
            }

            if (previewErrors.Count > 0)
            {
                return Results.ValidationProblem(previewErrors);
            }

            var document = await labelizeClient.RenderAsync(payload.Language, payload.Content, template.Dimensions, request.OutputType, cancellationToken);
            return Results.File(document.Content, document.ContentType);
        });

        return endpoints;
    }


    private static LabelTemplateDetailsResponse MapDetails(LabelTemplate template) =>
        new(
            template.Id,
            template.Name,
            template.SourceLanguage,
            template.Version,
            template.Description,
            template.RawContent,
            new LabelDimensionsDto(template.Dimensions.WidthMm, template.Dimensions.HeightMm, template.Dimensions.Dpmm),
            template.Variables
                .OrderBy(variable => variable.Order)
                .Select(variable => new TemplateVariableDto(variable.Name, variable.DisplayName, variable.IsRequired, variable.DefaultValue, variable.ExampleValue, variable.Order))
                .ToArray());

    public sealed record CreateOrUpdateTemplateRequest(
        string Name,
        string? Description,
        PrintLanguage SourceLanguage,
        string RawContent,
        LabelDimensionsDto? Dimensions,
        TemplateVariableDto[]? Variables);

    public sealed record RenderTemplateRequest(
        PreviewOutputType OutputType,
        Dictionary<string, string?>? VariableValues);

    public sealed record LabelTemplateSummaryResponse(
        Guid Id,
        string Name,
        PrintLanguage SourceLanguage,
        int Version);

    public sealed record LabelTemplateDetailsResponse(
        Guid Id,
        string Name,
        PrintLanguage SourceLanguage,
        int Version,
        string? Description,
        string RawContent,
        LabelDimensionsDto Dimensions,
        TemplateVariableDto[] Variables);

    public sealed record LabelDimensionsDto(
        double WidthMm,
        double HeightMm,
        int Dpmm);

    public sealed record TemplateVariableDto(
        string Name,
        string? DisplayName,
        bool IsRequired,
        string? DefaultValue,
        string? ExampleValue,
        int Order);
}
