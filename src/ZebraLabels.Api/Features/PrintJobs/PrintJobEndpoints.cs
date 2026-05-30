using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ZebraLabels.Api.Domain;
using ZebraLabels.Api.Infrastructure.Persistence;
using ZebraLabels.Api.Infrastructure.Printing;
using ZebraLabels.Api.Infrastructure.Rendering;

namespace ZebraLabels.Api.Features.PrintJobs;

public static class PrintJobEndpoints
{
    public static IEndpointRouteBuilder MapPrintJobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/print-jobs").WithTags("PrintJobs");

        group.MapGet("/", async (ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
            TypedResults.Ok((await dbContext.PrintJobs
                .AsNoTracking()
                .OrderByDescending(job => job.SubmittedAtUtc)
                .ToListAsync(cancellationToken))
                .Select(MapResponse)
                .ToList()));

        group.MapGet("/{printJobId:guid}", async (Guid printJobId, ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var job = await dbContext.PrintJobs.AsNoTracking().SingleOrDefaultAsync(value => value.Id == printJobId, cancellationToken);
            return job is null ? Results.NotFound() : Results.Ok(MapResponse(job));
        });

        group.MapPost("/prepare", async (PreparePrintJobRequest request, ZebraLabelsDbContext dbContext, TemplateRenderingService renderingService, PayloadTransformationService transformationService, CancellationToken cancellationToken) =>
        {
            var printerProfile = await dbContext.PrinterProfiles.AsNoTracking().SingleOrDefaultAsync(value => value.Id == request.PrinterProfileId, cancellationToken);
            if (printerProfile is null)
            {
                return Results.NotFound();
            }

            var template = await dbContext.Templates
                .AsNoTracking()
                .Include(value => value.Variables)
                .SingleOrDefaultAsync(value => value.Id == request.TemplateId && !value.IsArchived, cancellationToken);

            if (template is null)
            {
                return Results.NotFound();
            }

            var renderResult = renderingService.TryRender(template, request.VariableValues ?? new Dictionary<string, string?>());
            if (renderResult.Errors.Count > 0)
            {
                return Results.ValidationProblem(renderResult.Errors);
            }

            var transformResult = transformationService.Resolve(printerProfile, renderResult.Payload!);
            if (transformResult.Errors.Count > 0)
            {
                return Results.ValidationProblem(transformResult.Errors);
            }

            var payload = transformResult.Payload!;
            return Results.Ok(new PreparePrintJobResponse(payload.Content, payload.Checksum, payload.Language));
        });

        group.MapPost("/", async (CreatePrintJobRequest request, ZebraLabelsDbContext dbContext, PayloadTransformationService transformationService, TimeProvider timeProvider, CancellationToken cancellationToken) =>
        {
            var errors = ValidateCreateRequest(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var printerProfile = await dbContext.PrinterProfiles.AsNoTracking().SingleOrDefaultAsync(value => value.Id == request.PrinterProfileId, cancellationToken);
            if (printerProfile is null)
            {
                return Results.NotFound();
            }

            if (request.TemplateId.HasValue)
            {
                var templateExists = await dbContext.Templates.AsNoTracking().AnyAsync(value => value.Id == request.TemplateId.Value && !value.IsArchived, cancellationToken);
                if (!templateExists)
                {
                    return Results.NotFound();
                }
            }

            var payload = new PrintPayload(
                request.Content,
                string.IsNullOrWhiteSpace(request.Checksum) ? Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Content))) : request.Checksum,
                request.RequestedLanguage);

            var transformResult = transformationService.Resolve(printerProfile, payload);
            if (transformResult.Errors.Count > 0)
            {
                return Results.ValidationProblem(transformResult.Errors);
            }

            var now = timeProvider.GetUtcNow();
            var job = new PrintJob
            {
                Id = Guid.NewGuid(),
                TemplateId = request.TemplateId,
                PrinterProfileId = request.PrinterProfileId,
                RequestedLanguage = request.RequestedLanguage,
                EffectiveLanguage = transformResult.Payload!.Language,
                Status = PrintJobStatus.Sent,
                Checksum = transformResult.Payload.Checksum,
                SubmittedAtUtc = now,
                SentAtUtc = now
            };

            dbContext.PrintJobs.Add(job);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/print-jobs/{job.Id}", MapResponse(job));
        });

        return endpoints;
    }

    private static Dictionary<string, string[]> ValidateCreateRequest(CreatePrintJobRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            errors["content"] = ["Content is required."];
        }

        return errors;
    }

    private static PrintJobResponse MapResponse(PrintJob job) =>
        new(
            job.Id,
            job.TemplateId,
            job.PrinterProfileId,
            job.RequestedLanguage,
            job.EffectiveLanguage,
            job.Status,
            job.FailureReason);

    public sealed record PreparePrintJobRequest(
        Guid TemplateId,
        Guid PrinterProfileId,
        Dictionary<string, string?>? VariableValues);

    public sealed record PreparePrintJobResponse(
        string Content,
        string Checksum,
        PrintLanguage EffectiveLanguage);

    public sealed record CreatePrintJobRequest(
        Guid? TemplateId,
        Guid PrinterProfileId,
        PrintLanguage RequestedLanguage,
        string Content,
        string? Checksum);

    public sealed record PrintJobResponse(
        Guid Id,
        Guid? TemplateId,
        Guid PrinterProfileId,
        PrintLanguage RequestedLanguage,
        PrintLanguage EffectiveLanguage,
        PrintJobStatus Status,
        string? FailureReason);
}
