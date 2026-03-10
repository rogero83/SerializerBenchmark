var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.Server>("server");

builder.AddViteApp("client", "../Client")
    .WithReference(server).WaitFor(server)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
