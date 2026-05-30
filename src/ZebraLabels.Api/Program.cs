
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ZebraLabels.Api.Features.Health;
using ZebraLabels.Api.Features.Preview;
using ZebraLabels.Api.Features.PrinterProfiles;
using ZebraLabels.Api.Features.PrintJobs;
using ZebraLabels.Api.Features.Templates;
using ZebraLabels.Api.Infrastructure.Persistence;
using ZebraLabels.Api.Infrastructure.Preview;
using ZebraLabels.Api.Infrastructure.Printing;
using ZebraLabels.Api.Infrastructure.Rendering;

namespace ZebraLabels.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        BuildApplication(args).Run();
    }

    public static WebApplication BuildApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddProblemDetails();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        builder.Services.AddOpenApi();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("spa", policy =>
            {
                var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"];
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.Configure<LabelizeOptions>(builder.Configuration.GetSection(LabelizeOptions.SectionName));
        builder.Services.AddDbContext<ZebraLabelsDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("zebralabels")
                ?? "Host=localhost;Port=5432;Database=zebralabels;Username=postgres;Password=postgres";

            options.UseNpgsql(connectionString);
        });
        builder.Services.AddScoped<TemplateRenderingService>();
        builder.Services.AddScoped<PayloadTransformationService>();
        builder.Services.AddScoped<PreviewRequestPolicy>();
        builder.Services.AddHttpClient<LabelizeClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<LabelizeOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        var app = builder.Build();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        app.UseCors("spa");

        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ZebraLabelsDbContext>();
            dbContext.Database.EnsureCreated();
        }

        app.MapTemplateEndpoints();
        app.MapPreviewEndpoints();
        app.MapPrinterProfileEndpoints();
        app.MapPrintJobEndpoints();
        app.MapHealthEndpoints();
        app.MapDefaultEndpoints();

        return app;
    }
}
