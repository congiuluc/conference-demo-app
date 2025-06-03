using System.Linq;

namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents the status of a session
/// </summary>
public enum SessionStatus
{
    /// <summary>
    /// Session has been proposed but not reviewed
    /// </summary>
    Proposed,
    
    /// <summary>
    /// Session is being reviewed
    /// </summary>
    UnderReview,
    
    /// <summary>
    /// Session has been accepted for the conference
    /// </summary>
    Accepted,
    
    /// <summary>
    /// Session has been rejected
    /// </summary>
    Rejected,
    
    /// <summary>
    /// Session has been confirmed and scheduled
    /// </summary>
    Scheduled,
    
    /// <summary>
    /// Session has been cancelled
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Session is complete
    /// </summary>
    Completed
}

/// <summary>
/// Represents a session or talk at the tech conference
/// </summary>
public class Session : BaseEntity
{
    public Session()
    {
        PartitionKey = "Session";
        Status = SessionStatus.Proposed;
    }
    
    /// <summary>
    /// ID of the conference this session is associated with
    /// </summary>
    public string ConferenceId { get; set; } = default!;
    
    /// <summary>
    /// ID of the call for paper this session was submitted to (optional)
    /// </summary>
    public string? CallForPaperId { get; set; }
    
    /// <summary>
    /// Title of the session
    /// </summary>
    public string Title { get; set; } = default!;
    
    /// <summary>
    /// Description of the session content
    /// </summary>
    public string Description { get; set; } = default!;
    
    /// <summary>
    /// Start time of the session
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// End time of the session
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// Session track (e.g., "Frontend", "Backend", "DevOps")
    /// </summary>
    public string Track { get; set; } = default!;
    
    /// <summary>
    /// References to speaker IDs presenting this session
    /// </summary>
    public List<string> SpeakerIds { get; set; } = new List<string>();
    
    /// <summary>
    /// Technical level (Beginner, Intermediate, Advanced)
    /// </summary>
    public string Level { get; set; } = default!;
    
    /// <summary>
    /// Tags or topics covered in the session
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
    
    /// <summary>
    /// Current status of the session
    /// </summary>
    public SessionStatus Status { get; set; }
    
    /// <summary>
    /// Maximum number of attendees
    /// </summary>
    public int? MaxAttendees { get; set; }
    
    /// <summary>
    /// Feedback or notes from reviewers (internal)
    /// </summary>
    public string? ReviewNotes { get; set; }
      /// <summary>
    /// Type of session (e.g., "Talk", "Workshop", "Panel")
    /// </summary>
    public string SessionType { get; set; } = default!;

    // Compatibility properties for frontend views
    /// <summary>
    /// Duration in minutes - compatibility property
    /// </summary>
    public int Duration
    {
        get => (int)(EndTime - StartTime).TotalMinutes;
        set => EndTime = StartTime.AddMinutes(value);
    }

    /// <summary>
    /// Location of the session - compatibility property
    /// </summary>
    public string? Location { get; set; }    /// <summary>
    /// Single speaker ID - compatibility property for views that expect one speaker
    /// </summary>
    public string? SpeakerId
    {
        get => SpeakerIds.FirstOrDefault();
        set
        {
            SpeakerIds.Clear();
            if (!string.IsNullOrEmpty(value))
                SpeakerIds.Add(value);
        }
    }

    /// <summary>
    /// Scheduled start time - compatibility property
    /// </summary>
    public DateTime? ScheduledStartTime
    {
        get => StartTime;
        set => StartTime = value ?? DateTime.MinValue;
    }

    /// <summary>
    /// Room - compatibility property for Location
    /// </summary>
    public string? Room
    {
        get => Location;
        set => Location = value;
    }

    /// <summary>
    /// Type - compatibility property for SessionType
    /// </summary>
    public string Type
    {
        get => SessionType;
        set => SessionType = value;
    }
}
