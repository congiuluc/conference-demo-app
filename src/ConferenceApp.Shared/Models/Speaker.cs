namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents a speaker at tech conferences
/// </summary>
public class Speaker : BaseEntity
{
    public Speaker()
    {
        PartitionKey = "Speaker";
    }

    /// <summary>
    /// IDs of conferences this speaker participates in
    /// </summary>
    public List<string> ConferenceIds { get; set; } = new List<string>();
    
    /// <summary>
    /// Full name of the speaker
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Bio information about the speaker
    /// </summary>
    public string Bio { get; set; } = default!;
    
    /// <summary>
    /// Company or organization the speaker works for
    /// </summary>
    public string Company { get; set; } = default!;
    
    /// <summary>
    /// Job title of the speaker
    /// </summary>
    public string JobTitle { get; set; } = default!;
    
    /// <summary>
    /// URL to the speaker's profile photo
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Email address for contact
    /// </summary>
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Speaker's personal website or blog
    /// </summary>
    public string? Website { get; set; }
      /// <summary>
    /// Social media profiles
    /// </summary>
    public Dictionary<string, string>? SocialMedia { get; set; }

    // Compatibility properties for frontend views
    /// <summary>
    /// Job title - compatibility property for Title field
    /// </summary>
    public string Title
    {
        get => JobTitle;
        set => JobTitle = value;
    }

    /// <summary>
    /// Phone number - compatibility property
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Twitter handle - compatibility property
    /// </summary>
    public string? TwitterHandle
    {
        get => SocialMedia?.GetValueOrDefault("Twitter");
        set
        {
            SocialMedia ??= new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(value))
                SocialMedia["Twitter"] = value;
            else
                SocialMedia.Remove("Twitter");
        }
    }

    /// <summary>
    /// LinkedIn profile URL - compatibility property
    /// </summary>
    public string? LinkedInProfile
    {
        get => SocialMedia?.GetValueOrDefault("LinkedIn");
        set
        {
            SocialMedia ??= new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(value))
                SocialMedia["LinkedIn"] = value;
            else
                SocialMedia.Remove("LinkedIn");
        }
    }
}
