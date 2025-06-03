using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Attendee endpoints
/// </summary>
public class AttendeeEndpoints : IEndpoints
{    /// <summary>
    /// Maps attendee endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/attendees")
            .WithTags("Attendees")
            .WithOpenApi();

        // Get all attendees
        group.MapGet("/", GetAllAttendeesAsync)
            .WithName("GetAttendees")
            .WithDescription("Get all attendees")
            .Produces<IEnumerable<Attendee>>(StatusCodes.Status200OK);
            
        // Get attendees by conference
        group.MapGet("/conference/{conferenceId}", GetAttendeesByConferenceAsync)
            .WithName("GetAttendeesByConference")
            .WithDescription("Get attendees for a specific conference")
            .Produces<IEnumerable<Attendee>>(StatusCodes.Status200OK);

        // Get attendee by ID
        group.MapGet("/{id}", GetAttendeeByIdAsync)
            .WithName("GetAttendeeById")
            .WithDescription("Get an attendee by ID")
            .Produces<Attendee>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Get attendees by session ID
        group.MapGet("/session/{sessionId}", GetAttendeesBySessionAsync)
            .WithName("GetAttendeesBySession")
            .WithDescription("Get attendees registered for a session")
            .Produces<IEnumerable<Attendee>>(StatusCodes.Status200OK);

        // Create new attendee
        group.MapPost("/", CreateAttendeeAsync)
            .WithName("CreateAttendee")
            .WithDescription("Create a new attendee")
            .Produces<Attendee>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update attendee
        group.MapPut("/{id}", UpdateAttendeeAsync)
            .WithName("UpdateAttendee")
            .WithDescription("Update an existing attendee")
            .Produces<Attendee>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete attendee
        group.MapDelete("/{id}", DeleteAttendeeAsync)
            .WithName("DeleteAttendee")
            .WithDescription("Delete an attendee")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // Register attendee for session
        group.MapPost("/{id}/register/{sessionId}", RegisterForSessionAsync)
            .WithName("RegisterForSession")
            .WithDescription("Register attendee for a session")
            .Produces<Attendee>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Unregister attendee from session
        group.MapPost("/{id}/unregister/{sessionId}", UnregisterFromSessionAsync)
            .WithName("UnregisterFromSession")
            .WithDescription("Unregister attendee from a session")
            .Produces<Attendee>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }    /// <summary>
    /// Get all attendees
    /// </summary>
    private async Task<IResult> GetAllAttendeesAsync(ICosmosDbService<Attendee> cosmosDbService)
    {
        var attendees = await cosmosDbService.GetItemsAsync("Attendee");
        return Results.Ok(attendees);
    }
      /// <summary>
    /// Get attendees by conference ID
    /// </summary>
    private async Task<IResult> GetAttendeesByConferenceAsync(
        string conferenceId, 
        ICosmosDbService<Attendee> cosmosDbService)
    {
        var attendees = await cosmosDbService.QueryItemsAsync(
            a => a.ConferenceRegistrations.ContainsKey(conferenceId),
            "Attendee");
            
        return Results.Ok(attendees);
    }

    /// <summary>
    /// Get attendee by ID
    /// </summary>
    private async Task<IResult> GetAttendeeByIdAsync(string id, ICosmosDbService<Attendee> cosmosDbService)
    {
        var attendee = await cosmosDbService.GetItemAsync(id, "Attendee");

        if (attendee == null)
            return Results.NotFound();

        return Results.Ok(attendee);
    }    /// <summary>
    /// Get attendees registered for a session
    /// </summary>
    private async Task<IResult> GetAttendeesBySessionAsync(
        string sessionId, 
        ICosmosDbService<Attendee> cosmosDbService,
        ICosmosDbService<Session> sessionService)
    {
        // First get the session to determine which conference it belongs to
        var session = await sessionService.GetItemAsync(sessionId, "Session");
        
        if (session == null)
            return Results.NotFound("Session not found");
            
        string conferenceId = session.ConferenceId;
        
        // Query attendees who have registered for this session in this conference
        var attendees = await cosmosDbService.QueryItemsAsync(
            a => a.ConferenceRegistrations.ContainsKey(conferenceId) && 
                 a.ConferenceRegistrations[conferenceId].Contains(sessionId),
            "Attendee");
            
        return Results.Ok(attendees);
    }    /// <summary>
    /// Create a new attendee
    /// </summary>
    private async Task<IResult> CreateAttendeeAsync(
        Attendee attendee, 
        ICosmosDbService<Attendee> cosmosDbService,
        IValidator<Attendee> validator)
    {
        // First check if an attendee with the same email already exists
        var existingAttendees = await cosmosDbService.QueryItemsAsync(
            a => a.Email == attendee.Email,
            "Attendee");
            
        if (existingAttendees.Any())
            return Results.BadRequest($"An attendee with email {attendee.Email} already exists");
            
        var validationResult = await validator.ValidateAsync(attendee);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(attendee);
        return Results.Created($"/api/attendees/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing attendee
    /// </summary>
    private async Task<IResult> UpdateAttendeeAsync(
        string id, 
        Attendee attendee, 
        ICosmosDbService<Attendee> cosmosDbService,
        IValidator<Attendee> validator)
    {
        var existingAttendee = await cosmosDbService.GetItemAsync(id, "Attendee");
        
        if (existingAttendee == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        attendee.Id = id;
        attendee.PartitionKey = "Attendee";
        
        var validationResult = await validator.ValidateAsync(attendee);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, attendee);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete an attendee
    /// </summary>
    private async Task<IResult> DeleteAttendeeAsync(string id, ICosmosDbService<Attendee> cosmosDbService)
    {
        var attendee = await cosmosDbService.GetItemAsync(id, "Attendee");
        
        if (attendee == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "Attendee");
        return Results.NoContent();
    }    /// <summary>
    /// Register attendee for a session
    /// </summary>
    private async Task<IResult> RegisterForSessionAsync(
        string id, 
        string sessionId, 
        ICosmosDbService<Attendee> cosmosDbService,
        ICosmosDbService<Session> sessionService)
    {
        var attendee = await cosmosDbService.GetItemAsync(id, "Attendee");
        
        if (attendee == null)
            return Results.NotFound("Attendee not found");
            
        var session = await sessionService.GetItemAsync(sessionId, "Session");
        
        if (session == null)
            return Results.NotFound("Session not found");
            
        string conferenceId = session.ConferenceId;
        
        // Initialize conference registration list if not already exists
        if (!attendee.ConferenceRegistrations.ContainsKey(conferenceId))
        {
            attendee.ConferenceRegistrations[conferenceId] = new List<string>();
        }
        
        // Check if already registered for this session in this conference
        if (!attendee.ConferenceRegistrations[conferenceId].Contains(sessionId))
        {
            attendee.ConferenceRegistrations[conferenceId].Add(sessionId);
            var result = await cosmosDbService.UpdateItemAsync(id, attendee);
            return Results.Ok(result);
        }
        
        return Results.Ok(attendee);
    }    /// <summary>
    /// Unregister attendee from a session
    /// </summary>
    private async Task<IResult> UnregisterFromSessionAsync(
        string id, 
        string sessionId, 
        ICosmosDbService<Attendee> cosmosDbService,
        ICosmosDbService<Session> sessionService)
    {
        var attendee = await cosmosDbService.GetItemAsync(id, "Attendee");
        
        if (attendee == null)
            return Results.NotFound("Attendee not found");
            
        var session = await sessionService.GetItemAsync(sessionId, "Session");
        
        if (session == null)
            return Results.NotFound("Session not found");
            
        string conferenceId = session.ConferenceId;
        
        // Check if registered for this conference and session
        if (attendee.ConferenceRegistrations.ContainsKey(conferenceId) && 
            attendee.ConferenceRegistrations[conferenceId].Contains(sessionId))
        {
            attendee.ConferenceRegistrations[conferenceId].Remove(sessionId);
            
            // If no more sessions for this conference, remove the conference entry
            if (attendee.ConferenceRegistrations[conferenceId].Count == 0)
            {
                attendee.ConferenceRegistrations.Remove(conferenceId);
            }
            
            var result = await cosmosDbService.UpdateItemAsync(id, attendee);
            return Results.Ok(result);
        }
        
        return Results.Ok(attendee);
    }
}
