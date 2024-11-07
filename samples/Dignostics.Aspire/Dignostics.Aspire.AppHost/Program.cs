var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Dignostics_Aspire_ApiService>("apiservice");

builder.AddProject<Projects.Dignostics_Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
