namespace ConferenceApp.Shared.Models;

/// <summary>
/// Base entity for all database models
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Partition key for Cosmos DB
    /// </summary>
    public string PartitionKey { get; set; } = default!;
    
    /// <summary>
    /// Date when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      /// <summary>
    /// Date when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Compatibility properties for frontend views
    /// <summary>
    /// Created date - compatibility property for CreatedDate field
    /// </summary>
    public DateTime CreatedDate
    {
        get => CreatedAt;
        set => CreatedAt = value;
    }

    /// <summary>
    /// Modified date - compatibility property for ModifiedDate field
    /// </summary>
    public DateTime? ModifiedDate
    {
        get => UpdatedAt;
        set => UpdatedAt = value;
    }
}
