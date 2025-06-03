using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Session endpoints
/// </summary>
public class SessionEndpoints : IEndpoints
{    /// <summary>
    /// Maps session endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/sessions")
            .WithTags("Sessions")
            .WithOpenApi();

        // Get all sessions
        group.MapGet("/", GetAllSessionsAsync)
            .WithName("GetSessions")
            .WithDescription("Get all sessions")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);
            
        // Get sessions by conference
        group.MapGet("/conference/{conferenceId}", GetSessionsByConferenceAsync)
            .WithName("GetSessionsByConference")
            .WithDescription("Get sessions for a specific conference")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);

        // Get session by ID
        group.MapGet("/{id}", GetSessionByIdAsync)
            .WithName("GetSessionById")
            .WithDescription("Get a session by ID")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Get sessions by track
        group.MapGet("/track/{track}", GetSessionsByTrackAsync)
            .WithName("GetSessionsByTrack")
            .WithDescription("Get sessions by track")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);

        // Get sessions by tag
        group.MapGet("/tag/{tag}", GetSessionsByTagAsync)
            .WithName("GetSessionsByTag")
            .WithDescription("Get sessions by tag")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);

        // Get sessions by speaker
        group.MapGet("/speaker/{speakerId}", GetSessionsBySpeakerAsync)
            .WithName("GetSessionsBySpeaker")
            .WithDescription("Get sessions by speaker ID")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);

        // Create new session
        group.MapPost("/", CreateSessionAsync)
            .WithName("CreateSession")
            .WithDescription("Create a new session")
            .Produces<Session>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update session
        group.MapPut("/{id}", UpdateSessionAsync)
            .WithName("UpdateSession")
            .WithDescription("Update an existing session")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete session
        group.MapDelete("/{id}", DeleteSessionAsync)
            .WithName("DeleteSession")
            .WithDescription("Delete a session")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }    /// <summary>
    /// Get all sessions
    /// </summary>
    private async Task<IResult> GetAllSessionsAsync(ICosmosDbService<Session> cosmosDbService)
    {
        var sessions = await cosmosDbService.GetItemsAsync("Session");
        return Results.Ok(sessions);
    }
    
    /// <summary>
    /// Get sessions by conference ID
    /// </summary>
    private async Task<IResult> GetSessionsByConferenceAsync(
        string conferenceId, 
        ICosmosDbService<Session> cosmosDbService)
    {
        var sessions = await cosmosDbService.QueryItemsAsync(
            s => s.ConferenceId == conferenceId,
            "Session");
            
        return Results.Ok(sessions);
    }

    /// <summary>
    /// Get session by ID
    /// </summary>
    private async Task<IResult> GetSessionByIdAsync(string id, ICosmosDbService<Session> cosmosDbService)
    {
        var session = await cosmosDbService.GetItemAsync(id, "Session");

        if (session == null)
            return Results.NotFound();

        return Results.Ok(session);
    }

    /// <summary>
    /// Get sessions by track
    /// </summary>
    private async Task<IResult> GetSessionsByTrackAsync(string track, ICosmosDbService<Session> cosmosDbService)
    {
        var sessions = await cosmosDbService.QueryItemsAsync(
            s => s.Track.Equals(track, StringComparison.OrdinalIgnoreCase),
            "Session");
            
        return Results.Ok(sessions);
    }

    /// <summary>
    /// Get sessions by tag
    /// </summary>
    private async Task<IResult> GetSessionsByTagAsync(string tag, ICosmosDbService<Session> cosmosDbService)
    {
        var sessions = await cosmosDbService.QueryItemsAsync(
            s => s.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)),
            "Session");
            
        return Results.Ok(sessions);
    }

    /// <summary>
    /// Get sessions by speaker ID
    /// </summary>
    private async Task<IResult> GetSessionsBySpeakerAsync(string speakerId, ICosmosDbService<Session> cosmosDbService)
    {
        var sessions = await cosmosDbService.QueryItemsAsync(
            s => s.SpeakerIds.Contains(speakerId),
            "Session");
            
        return Results.Ok(sessions);
    }

    /// <summary>
    /// Create a new session
    /// </summary>
    private async Task<IResult> CreateSessionAsync(
        Session session, 
        ICosmosDbService<Session> cosmosDbService,
        IValidator<Session> validator)
    {
        var validationResult = await validator.ValidateAsync(session);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(session);
        return Results.Created($"/api/sessions/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing session
    /// </summary>
    private async Task<IResult> UpdateSessionAsync(
        string id, 
        Session session, 
        ICosmosDbService<Session> cosmosDbService,
        IValidator<Session> validator)
    {
        var existingSession = await cosmosDbService.GetItemAsync(id, "Session");
        
        if (existingSession == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        session.Id = id;
        session.PartitionKey = "Session";
        
        var validationResult = await validator.ValidateAsync(session);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, session);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete a session
    /// </summary>
    private async Task<IResult> DeleteSessionAsync(string id, ICosmosDbService<Session> cosmosDbService)
    {
        var session = await cosmosDbService.GetItemAsync(id, "Session");
        
        if (session == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "Session");
        return Results.NoContent();
    }
}
