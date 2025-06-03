namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents a venue for conferences
/// </summary>
public class Venue : BaseEntity
{
    public Venue()
    {
        PartitionKey = "Venue";
    }
    
    /// <summary>
    /// IDs of conferences that use this venue
    /// </summary>
    public List<string> ConferenceIds { get; set; } = new List<string>();
    
    /// <summary>
    /// Name of the venue
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Address of the venue
    /// </summary>
    public string Address { get; set; } = default!;
    
    /// <summary>
    /// City where the venue is located
    /// </summary>
    public string City { get; set; } = default!;
    
    /// <summary>
    /// State or province where the venue is located
    /// </summary>
    public string State { get; set; } = default!;
    
    /// <summary>
    /// Zip or postal code
    /// </summary>
    public string ZipCode { get; set; } = default!;
    
    /// <summary>
    /// Country where the venue is located
    /// </summary>
    public string Country { get; set; } = default!;
    
    /// <summary>
    /// Venue capacity or maximum attendees
    /// </summary>
    public int Capacity { get; set; }
    
    /// <summary>
    /// List of rooms or halls available at this venue
    /// </summary>
    public List<Room> Rooms { get; set; } = new List<Room>();
}

/// <summary>
/// Represents a room within a venue
/// </summary>
public class Room
{
    /// <summary>
    /// Unique identifier for the room
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Name or number of the room
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Capacity of the room (maximum number of attendees)
    /// </summary>
    public int Capacity { get; set; }
    
    /// <summary>
    /// Equipment available in the room (e.g., "Projector", "Microphone")
    /// </summary>
    public List<string> Equipment { get; set; } = new List<string>();
}
