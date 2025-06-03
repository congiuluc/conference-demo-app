using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Session management endpoints for reviewing and managing session proposals
/// </summary>
public class SessionManagementEndpoints : IEndpoints
{
    /// <summary>
    /// Maps session management endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/sessionmanagement")
            .WithTags("Session Management")
            .WithOpenApi();

        // Get sessions by status
        group.MapGet("/status/{status}", GetSessionsByStatusAsync)
            .WithName("GetSessionsByStatus")
            .WithDescription("Get sessions by status")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);
            
        // Get sessions by conference and status
        group.MapGet("/conference/{conferenceId}/status/{status}", GetSessionsByConferenceAndStatusAsync)
            .WithName("GetSessionsByConferenceAndStatus")
            .WithDescription("Get sessions by conference and status")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);
            
        // Update session status
        group.MapPut("/{id}/status/{status}", UpdateSessionStatusAsync)
            .WithName("UpdateSessionStatus")
            .WithDescription("Update session status")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            
        // Add review notes to session
        group.MapPut("/{id}/review", AddReviewNotesToSessionAsync)
            .WithName("AddReviewNotesToSession")
            .WithDescription("Add review notes to session")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
    
    /// <summary>
    /// Get sessions by status
    /// </summary>
    private async Task<IResult> GetSessionsByStatusAsync(
        string status, 
        ICosmosDbService<Session> cosmosDbService)
    {
        if (!Enum.TryParse<SessionStatus>(status, true, out var sessionStatus))
        {
            return Results.BadRequest($"Invalid status: {status}");
        }
        
        var sessions = await cosmosDbService.QueryItemsAsync(
            s => s.Status == sessionStatus,
            "Session");
            
        return Results.Ok(sessions);
    }
    
    /// <summary>
    /// Get sessions by conference and status
    /// </summary>
    private async Task<IResult> GetSessionsByConferenceAndStatusAsync(
        string conferenceId, 
        string status, 
        ICosmosDbService<Session> cosmosDbService)
    {
        if (!Enum.TryParse<SessionStatus>(status, true, out var sessionStatus))
        {
            return Results.BadRequest($"Invalid status: {status}");
        }
        
        var sessions = await cosmosDbService.QueryItemsAsync(
            s => s.ConferenceId == conferenceId && s.Status == sessionStatus,
            "Session");
            
        return Results.Ok(sessions);
    }
    
    /// <summary>
    /// Update session status
    /// </summary>
    private async Task<IResult> UpdateSessionStatusAsync(
        string id, 
        string status, 
        ICosmosDbService<Session> cosmosDbService)
    {
        if (!Enum.TryParse<SessionStatus>(status, true, out var sessionStatus))
        {
            return Results.BadRequest($"Invalid status: {status}");
        }
        
        var session = await cosmosDbService.GetItemAsync(id, "Session");
        
        if (session == null)
            return Results.NotFound();
            
        session.Status = sessionStatus;
        var result = await cosmosDbService.UpdateItemAsync(id, session);
        return Results.Ok(result);
    }
    
    /// <summary>
    /// Add review notes to session
    /// </summary>
    private async Task<IResult> AddReviewNotesToSessionAsync(
        string id, 
        ReviewNotesRequest request, 
        ICosmosDbService<Session> cosmosDbService)
    {
        var session = await cosmosDbService.GetItemAsync(id, "Session");
        
        if (session == null)
            return Results.NotFound();
            
        session.ReviewNotes = request.Notes;
        
        // If status is specified, update it
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse<SessionStatus>(request.Status, true, out var sessionStatus))
            {
                session.Status = sessionStatus;
            }
        }
        
        var result = await cosmosDbService.UpdateItemAsync(id, session);
        return Results.Ok(result);
    }
}

/// <summary>
/// Request model for adding review notes
/// </summary>
public class ReviewNotesRequest
{
    /// <summary>
    /// Review notes
    /// </summary>
    public string Notes { get; set; } = default!;
    
    /// <summary>
    /// Optional status update
    /// </summary>
    public string? Status { get; set; }
}
