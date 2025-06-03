namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents a tech conference
/// </summary>
public class Conference : BaseEntity
{
    public Conference()
    {
        PartitionKey = "Conference";
    }
    
    /// <summary>
    /// Name of the conference
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Description of the conference
    /// </summary>
    public string Description { get; set; } = default!;
    
    /// <summary>
    /// Start date of the conference
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// End date of the conference
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Conference website URL
    /// </summary>
    public string? Website { get; set; }
    
    /// <summary>
    /// Logo URL for the conference
    /// </summary>
    public string? LogoUrl { get; set; }
    
    /// <summary>
    /// Flag to indicate if the conference is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Categories or themes of the conference
    /// </summary>
    public List<string> Categories { get; set; } = new List<string>();
    
    /// <summary>
    /// Venues associated with this conference
    /// </summary>
    public List<string> VenueIds { get; set; } = new List<string>();

    // Compatibility properties for frontend views
    public string? Location { get; set; } // Add location property
    public int MaxAttendees { get; set; } = 0; // Add max attendees
    public int CurrentAttendees { get; set; } = 0; // Add current attendees count
    public string? Organizer { get; set; } // Add organizer property
}
