using Microsoft.Azure.Cosmos;

namespace ConferenceApp.API.Services;

/// <summary>
/// Cosmos DB client configuration
/// </summary>
public class CosmosDbConfig
{
    /// <summary>
    /// Cosmos DB endpoint URL
    /// </summary>
    public string EndpointUrl { get; set; } = default!;
    
    /// <summary>
    /// Primary key for authentication
    /// </summary>
    public string PrimaryKey { get; set; } = default!;
    
    /// <summary>
    /// Database name
    /// </summary>
    public string DatabaseName { get; set; } = default!;
    
    /// <summary>
    /// Container name
    /// </summary>
    public string ContainerName { get; set; } = default!;
}
