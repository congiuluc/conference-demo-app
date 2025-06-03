using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


// Add Application Insights for monitoring (optional)
// var appInsights = builder.AddAzureApplicationInsights("appinsights");

// Add the API project with dependencies
var api = builder.AddProject<Projects.ConferenceApp_API>("conferenceapp-api")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);
    // .WithReference(appInsights); // Uncomment when using Application Insights

// Add the UI project with reference to the API
var ui = builder.AddProject<Projects.ConferenceApp_UI>("conferenceapp-ui")
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithEnvironment("ApiSettings__BaseUrl", api.GetEndpoint("https"));
    // .WithReference(appInsights); // Uncomment when using Application Insights

// Build and run the application
var app = builder.Build();

await app.RunAsync();