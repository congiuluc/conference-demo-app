namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents an agenda or schedule for a conference day
/// </summary>
public class AgendaDay : BaseEntity
{
    public AgendaDay()
    {
        PartitionKey = "AgendaDay";
    }
    
    /// <summary>
    /// ID of the conference this agenda is associated with
    /// </summary>
    public string ConferenceId { get; set; } = default!;
    
    /// <summary>
    /// Date this agenda covers
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Title or name of this day (e.g., "Opening Day", "Day 1", etc.)
    /// </summary>
    public string Title { get; set; } = default!;
    
    /// <summary>
    /// Description or theme of this day
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Time slots for this agenda day, organized by track and/or venue
    /// Key is the track name or venue+room combination, value is the list of time slots
    /// </summary>
    public Dictionary<string, List<AgendaTimeSlot>> TimeSlotsByTrack { get; set; } = new Dictionary<string, List<AgendaTimeSlot>>();
}

/// <summary>
/// Represents a time slot in the conference agenda
/// </summary>
public class AgendaTimeSlot
{
    /// <summary>
    /// Session ID associated with this time slot (null for breaks or non-session events)
    /// </summary>
    public string? SessionId { get; set; }
    
    /// <summary>
    /// Title of this time slot (used for breaks or when no session is linked)
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Start time of this slot
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// End time of this slot
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// Type of time slot (e.g., "Session", "Break", "Keynote", "Registration")
    /// </summary>
    public string SlotType { get; set; } = default!;
    
    /// <summary>
    /// Venue ID for this time slot
    /// </summary>
    public string? VenueId { get; set; }
    
    /// <summary>
    /// Room or location within the venue
    /// </summary>
    public string? Room { get; set; }
    
    /// <summary>
    /// Additional information or notes about this time slot
    /// </summary>
    public string? Notes { get; set; }
}
