namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents an attendee of tech conferences
/// </summary>
public class Attendee : BaseEntity
{
    public Attendee()
    {
        PartitionKey = "Attendee";
    }
    
    /// <summary>
    /// Dictionary mapping conference IDs to the sessions registered for each
    /// </summary>
    public Dictionary<string, List<string>> ConferenceRegistrations { get; set; } = new Dictionary<string, List<string>>();
    
    /// <summary>
    /// Full name of the attendee
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Email address for contact (unique identifier across conferences)
    /// </summary>
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Company or organization the attendee works for
    /// </summary>
    public string? Company { get; set; }
    
    /// <summary>
    /// Job title of the attendee
    /// </summary>
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Initial registration date
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Compatibility properties for frontend views
    public string FirstName 
    { 
        get 
        {
            var parts = Name?.Split(' ') ?? Array.Empty<string>();
            return parts.Length > 0 ? parts[0] : string.Empty;
        }
        set 
        {
            var parts = Name?.Split(' ') ?? Array.Empty<string>();
            if (parts.Length > 1)
                Name = $"{value} {string.Join(" ", parts.Skip(1))}";
            else
                Name = value ?? string.Empty;
        }
    }

    public string LastName 
    { 
        get 
        {
            var parts = Name?.Split(' ') ?? Array.Empty<string>();
            return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
        }
        set 
        {
            var parts = Name?.Split(' ') ?? Array.Empty<string>();
            if (parts.Length > 0)
                Name = $"{parts[0]} {value}";
            else
                Name = $"Unknown {value}";
        }
    }

    public string? Phone { get; set; } // Add phone property
    public string? Bio { get; set; } // Add bio property
    public string? ConferenceId { get; set; } // Add single conference ID for compatibility
    public bool IsConfirmed { get; set; } = false; // Add confirmation status
}
