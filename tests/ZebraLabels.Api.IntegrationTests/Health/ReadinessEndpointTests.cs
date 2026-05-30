using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace ZebraLabels.Api.IntegrationTests.Health;

public sealed class ReadinessEndpointTests : IClassFixture<ReadinessEndpointTests.TestApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public ReadinessEndpointTests(TestApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetReady_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/health/ready");

        response.EnsureSuccessStatusCode();
    }

    public sealed class TestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            return base.CreateHost(builder);
        }
    }
}
