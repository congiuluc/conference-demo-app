namespace ConferenceApp.Shared.Models;

/// <summary>
/// Represents a call for papers (CFP) for a conference
/// </summary>
public class CallForPaper : BaseEntity
{
    public CallForPaper()
    {
        PartitionKey = "CallForPaper";
    }
    
    /// <summary>
    /// ID of the conference this call for papers is associated with
    /// </summary>
    public string ConferenceId { get; set; } = default!;
    
    /// <summary>
    /// Title of the call for papers
    /// </summary>
    public string Title { get; set; } = default!;
    
    /// <summary>
    /// Description of the call for papers
    /// </summary>
    public string Description { get; set; } = default!;
    
    /// <summary>
    /// Start date for call for papers submissions
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Deadline for call for papers submissions
    /// </summary>
    public DateTime Deadline { get; set; }
    
    /// <summary>
    /// Topics or areas of interest
    /// </summary>
    public List<string> Topics { get; set; } = new List<string>();
    
    /// <summary>
    /// Session types that can be submitted (e.g., "Talk", "Workshop", "Panel")
    /// </summary>
    public List<string> SessionTypes { get; set; } = new List<string>();
    
    /// <summary>
    /// Evaluation criteria for submissions
    /// </summary>
    public string? EvaluationCriteria { get; set; }
    
    /// <summary>
    /// Email address or contact for questions
    /// </summary>
    public string? ContactEmail { get; set; }
    
    /// <summary>
    /// URL to additional information
    /// </summary>
    public string? InfoUrl { get; set; }
    
    /// <summary>
    /// Whether the call for papers is currently open
    /// </summary>
    public bool IsOpen { get; set; } = true;
}
