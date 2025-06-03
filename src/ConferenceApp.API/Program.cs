using ConferenceApp.API.Services;
using ConferenceApp.API.Endpoints;
using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentValidation;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb").Get<CosmosDbConfig>() ?? new CosmosDbConfig
{
    EndpointUrl = builder.Configuration["CosmosDb:EndpointUrl"] ?? "https://localhost:8081",
    PrimaryKey = builder.Configuration["CosmosDb:PrimaryKey"] ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", // Default emulator key
    DatabaseName = builder.Configuration["CosmosDb:DatabaseName"] ?? "ConferenceDb",
    ContainerName = builder.Configuration["CosmosDb:ContainerName"] ?? "ConferenceContainer"
};

// Register CosmosDB client and services
builder.Services.AddSingleton(sp =>
{
    // Create a cosmos client instance
    var cosmosClient = new CosmosClient(cosmosDbConfig.EndpointUrl, cosmosDbConfig.PrimaryKey, new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });

    // Initialize database and container if they don't exist
    InitializeCosmosDbAsync(cosmosClient, cosmosDbConfig.DatabaseName, cosmosDbConfig.ContainerName).GetAwaiter().GetResult();
    
    return cosmosClient;
});

// Register the typed repositories
builder.Services.AddSingleton<ICosmosDbService<Conference>>(sp => new CosmosDbService<Conference>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));
    
builder.Services.AddSingleton<ICosmosDbService<Speaker>>(sp => new CosmosDbService<Speaker>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));
    
builder.Services.AddSingleton<ICosmosDbService<Session>>(sp => new CosmosDbService<Session>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));
    
builder.Services.AddSingleton<ICosmosDbService<Attendee>>(sp => new CosmosDbService<Attendee>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));
    
builder.Services.AddSingleton<ICosmosDbService<Venue>>(sp => new CosmosDbService<Venue>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));
    
builder.Services.AddSingleton<ICosmosDbService<CallForPaper>>(sp => new CosmosDbService<CallForPaper>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));
    
builder.Services.AddSingleton<ICosmosDbService<AgendaDay>>(sp => new CosmosDbService<AgendaDay>(
    sp.GetRequiredService<CosmosClient>(), 
    cosmosDbConfig.DatabaseName, 
    cosmosDbConfig.ContainerName));

// Register validators
builder.Services.AddScoped<IValidator<Conference>, ConferenceValidator>();
builder.Services.AddScoped<IValidator<Speaker>, SpeakerValidator>();
builder.Services.AddScoped<IValidator<Session>, SessionValidator>();
builder.Services.AddScoped<IValidator<Attendee>, AttendeeValidator>();
builder.Services.AddScoped<IValidator<Venue>, VenueValidator>();
builder.Services.AddScoped<IValidator<CallForPaper>, CallForPaperValidator>();
builder.Services.AddScoped<IValidator<AgendaDay>, AgendaDayValidator>();

// Register endpoints
builder.Services.AddScoped<IEndpoints, ConferenceEndpoints>();
builder.Services.AddScoped<IEndpoints, SpeakerEndpoints>();
builder.Services.AddScoped<IEndpoints, SessionEndpoints>();
builder.Services.AddScoped<IEndpoints, AttendeeEndpoints>();
builder.Services.AddScoped<IEndpoints, VenueEndpoints>();
builder.Services.AddScoped<IEndpoints, CallForPaperEndpoints>();
builder.Services.AddScoped<IEndpoints, AgendaEndpoints>();
builder.Services.AddScoped<IEndpoints, SessionManagementEndpoints>();

// Configure API and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Conference Management API",
        Description = "API for managing tech conference data",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Tech Conference Team",
            Email = "contact@techconference.com"
        }
    });
});

// Configure JSON options to handle cycles
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Configure JSON serialization to handle enums as strings
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Map the endpoints from all registered endpoint classes
using (var scope = app.Services.CreateScope())
{
    var endpointMappings = scope.ServiceProvider.GetServices<IEndpoints>();
    foreach (var endpointMapping in endpointMappings)
    {
        endpointMapping.MapEndpoints(app);
    }
}

app.Run();

// Helper method to initialize CosmosDB
async Task InitializeCosmosDbAsync(CosmosClient cosmosClient, string databaseName, string containerName)
{
    // Create database if it doesn't exist
    DatabaseResponse database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
    
    // Create container with a partition key if it doesn't exist
    await database.Database.CreateContainerIfNotExistsAsync(
        new ContainerProperties(containerName, "/partitionKey"));
}
