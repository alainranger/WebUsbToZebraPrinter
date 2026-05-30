using Microsoft.EntityFrameworkCore;
using ZebraLabels.Api.Domain;
using ZebraLabels.Api.Infrastructure.Persistence;

namespace ZebraLabels.Api.Features.PrinterProfiles;

public static class PrinterProfileEndpoints
{
    public static IEndpointRouteBuilder MapPrinterProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/printer-profiles").WithTags("PrinterProfiles");

        group.MapGet("/", async (ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
            TypedResults.Ok((await dbContext.PrinterProfiles
                .AsNoTracking()
                .OrderBy(profile => profile.Name)
                .ToListAsync(cancellationToken))
                .Select(MapResponse)
                .ToList()));

        group.MapPost("/", async (CreateOrUpdatePrinterProfileRequest request, ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var errors = ValidateRequest(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var profile = new PrinterProfile
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                VendorId = request.VendorId,
                ProductId = request.ProductId,
                PreferredLanguage = request.PreferredLanguage,
                Capabilities = new LanguageCapabilities(request.Capabilities.SupportsEpl, request.Capabilities.SupportsZpl, request.Capabilities.AllowEplToZplFallback),
                DefaultDimensions = new LabelDimensions(request.DefaultDimensions.WidthMm, request.DefaultDimensions.HeightMm, request.DefaultDimensions.Dpmm),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };

            dbContext.PrinterProfiles.Add(profile);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/printer-profiles/{profile.Id}", MapResponse(profile));
        });

        group.MapPut("/{printerProfileId:guid}", async (Guid printerProfileId, CreateOrUpdatePrinterProfileRequest request, ZebraLabelsDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var errors = ValidateRequest(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var profile = await dbContext.PrinterProfiles.SingleOrDefaultAsync(value => value.Id == printerProfileId, cancellationToken);
            if (profile is null)
            {
                return Results.NotFound();
            }

            profile.Name = request.Name.Trim();
            profile.VendorId = request.VendorId;
            profile.ProductId = request.ProductId;
            profile.PreferredLanguage = request.PreferredLanguage;
            profile.Capabilities = new LanguageCapabilities(request.Capabilities.SupportsEpl, request.Capabilities.SupportsZpl, request.Capabilities.AllowEplToZplFallback);
            profile.DefaultDimensions = new LabelDimensions(request.DefaultDimensions.WidthMm, request.DefaultDimensions.HeightMm, request.DefaultDimensions.Dpmm);
            profile.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(MapResponse(profile));
        });

        return endpoints;
    }

    private static Dictionary<string, string[]> ValidateRequest(CreateOrUpdatePrinterProfileRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Name is required."];
        }

        try
        {
            _ = new LabelDimensions(request.DefaultDimensions.WidthMm, request.DefaultDimensions.HeightMm, request.DefaultDimensions.Dpmm);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            errors[$"defaultDimensions.{exception.ParamName}"] = [exception.Message];
        }

        var capabilities = new LanguageCapabilities(request.Capabilities.SupportsEpl, request.Capabilities.SupportsZpl, request.Capabilities.AllowEplToZplFallback);
        if (!capabilities.Supports(request.PreferredLanguage))
        {
            errors["preferredLanguage"] = ["Preferred language must be supported by the printer capabilities."];
        }

        return errors;
    }

    private static PrinterProfileResponse MapResponse(PrinterProfile profile) =>
        new(
            profile.Id,
            profile.Name,
            profile.VendorId,
            profile.ProductId,
            profile.PreferredLanguage,
            new LanguageCapabilitiesDto(profile.Capabilities.SupportsEpl, profile.Capabilities.SupportsZpl, profile.Capabilities.AllowEplToZplFallback),
            new LabelDimensionsDto(profile.DefaultDimensions.WidthMm, profile.DefaultDimensions.HeightMm, profile.DefaultDimensions.Dpmm),
            profile.Notes);

    public sealed record CreateOrUpdatePrinterProfileRequest(
        string Name,
        int? VendorId,
        int? ProductId,
        PrintLanguage PreferredLanguage,
        LanguageCapabilitiesDto Capabilities,
        LabelDimensionsDto DefaultDimensions,
        string? Notes);

    public sealed record PrinterProfileResponse(
        Guid Id,
        string Name,
        int? VendorId,
        int? ProductId,
        PrintLanguage PreferredLanguage,
        LanguageCapabilitiesDto Capabilities,
        LabelDimensionsDto DefaultDimensions,
        string? Notes);

    public sealed record LanguageCapabilitiesDto(
        bool SupportsEpl,
        bool SupportsZpl,
        bool AllowEplToZplFallback);

    public sealed record LabelDimensionsDto(
        double WidthMm,
        double HeightMm,
        int Dpmm);
}
