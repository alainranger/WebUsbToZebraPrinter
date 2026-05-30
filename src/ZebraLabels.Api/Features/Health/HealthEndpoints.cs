namespace ZebraLabels.Api.Features.Health;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/ready", () => TypedResults.Ok(new { status = "ready" }))
            .WithName("GetReadiness");

        return endpoints;
    }
}
