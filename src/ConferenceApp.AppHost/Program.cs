var builder = DistributedApplication.CreateBuilder(args);

// Add the API project
var apiService = builder.AddProject("conferenceapp-api", "../ConferenceApp.API/ConferenceApp.API.csproj")
    .WithHttpEndpoint(port: 5001, name: "api-http");

// Add the UI project and reference the API service
var webApp = builder.AddProject("conferenceapp-ui", "../ConferenceApp.UI/ConferenceApp.UI.csproj")
    .WithHttpEndpoint(port: 5000, name: "web-http")
    .WithReference(apiService);

builder.Build().Run();