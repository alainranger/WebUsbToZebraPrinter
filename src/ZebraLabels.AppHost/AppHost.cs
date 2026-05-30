var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("zebralabels");

var labelize = builder.AddDockerfile("labelize", "..\\..\\docker\\labelize")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http");

var api = builder.AddProject<Projects.ZebraLabels_Api>("api")
    .WithReference(database)
    .WithEnvironment("Labelize__BaseUrl", labelize.GetEndpoint("http"))
    .WaitFor(database)
    .WaitFor(labelize)
    .WithExternalHttpEndpoints();

builder.AddViteApp("web", "..\\ZebraLabels.Web")
    .WithEnvironment("VITE_API_PROXY_TARGET", api.GetEndpoint("http"))
    .WaitFor(api);

builder.Build().Run();
