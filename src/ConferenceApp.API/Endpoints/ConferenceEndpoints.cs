using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Conference endpoints
/// </summary>
public class ConferenceEndpoints : IEndpoints
{
    /// <summary>
    /// Maps conference endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/conferences")
            .WithTags("Conferences")
            .WithOpenApi();

        // Get all conferences
        group.MapGet("/", GetAllConferencesAsync)
            .WithName("GetConferences")
            .WithDescription("Get all conferences")
            .Produces<IEnumerable<Conference>>(StatusCodes.Status200OK);

        // Get active conferences
        group.MapGet("/active", GetActiveConferencesAsync)
            .WithName("GetActiveConferences")
            .WithDescription("Get all active conferences")
            .Produces<IEnumerable<Conference>>(StatusCodes.Status200OK);

        // Get conference by ID
        group.MapGet("/{id}", GetConferenceByIdAsync)
            .WithName("GetConferenceById")
            .WithDescription("Get a conference by ID")
            .Produces<Conference>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Create new conference
        group.MapPost("/", CreateConferenceAsync)
            .WithName("CreateConference")
            .WithDescription("Create a new conference")
            .Produces<Conference>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update conference
        group.MapPut("/{id}", UpdateConferenceAsync)
            .WithName("UpdateConference")
            .WithDescription("Update an existing conference")
            .Produces<Conference>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete conference
        group.MapDelete("/{id}", DeleteConferenceAsync)
            .WithName("DeleteConference")
            .WithDescription("Delete a conference")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // Get speakers for a conference
        group.MapGet("/{id}/speakers", GetConferenceSpeakersAsync)
            .WithName("GetConferenceSpeakers")
            .WithDescription("Get all speakers for a specific conference")
            .Produces<IEnumerable<Speaker>>(StatusCodes.Status200OK);

        // Get sessions for a conference
        group.MapGet("/{id}/sessions", GetConferenceSessionsAsync)
            .WithName("GetConferenceSessions")
            .WithDescription("Get all sessions for a specific conference")
            .Produces<IEnumerable<Session>>(StatusCodes.Status200OK);

        // Get attendees for a conference
        group.MapGet("/{id}/attendees", GetConferenceAttendeesAsync)
            .WithName("GetConferenceAttendees")
            .WithDescription("Get all attendees for a specific conference")
            .Produces<IEnumerable<Attendee>>(StatusCodes.Status200OK);

        // Get venues for a conference
        group.MapGet("/{id}/venues", GetConferenceVenuesAsync)
            .WithName("GetConferenceVenues")
            .WithDescription("Get all venues for a specific conference")
            .Produces<IEnumerable<Venue>>(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Get all conferences
    /// </summary>
    private async Task<IResult> GetAllConferencesAsync(ICosmosDbService<Conference> cosmosDbService)
    {
        var conferences = await cosmosDbService.GetItemsAsync("Conference");
        return Results.Ok(conferences);
    }

    /// <summary>
    /// Get active conferences
    /// </summary>
    private async Task<IResult> GetActiveConferencesAsync(ICosmosDbService<Conference> cosmosDbService)
    {
        var conferences = await cosmosDbService.QueryItemsAsync(
            c => c.IsActive, 
            "Conference");
            
        return Results.Ok(conferences);
    }

    /// <summary>
    /// Get conference by ID
    /// </summary>
    private async Task<IResult> GetConferenceByIdAsync(string id, ICosmosDbService<Conference> cosmosDbService)
    {
        var conference = await cosmosDbService.GetItemAsync(id, "Conference");

        if (conference == null)
            return Results.NotFound();

        return Results.Ok(conference);
    }

    /// <summary>
    /// Create a new conference
    /// </summary>
    private async Task<IResult> CreateConferenceAsync(
        Conference conference, 
        ICosmosDbService<Conference> cosmosDbService,
        IValidator<Conference> validator)
    {
        var validationResult = await validator.ValidateAsync(conference);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(conference);
        return Results.Created($"/api/conferences/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing conference
    /// </summary>
    private async Task<IResult> UpdateConferenceAsync(
        string id, 
        Conference conference, 
        ICosmosDbService<Conference> cosmosDbService,
        IValidator<Conference> validator)
    {
        var existingConference = await cosmosDbService.GetItemAsync(id, "Conference");
        
        if (existingConference == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        conference.Id = id;
        conference.PartitionKey = "Conference";
        
        var validationResult = await validator.ValidateAsync(conference);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, conference);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete a conference
    /// </summary>
    private async Task<IResult> DeleteConferenceAsync(string id, ICosmosDbService<Conference> cosmosDbService)
    {
        var conference = await cosmosDbService.GetItemAsync(id, "Conference");
        
        if (conference == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "Conference");
        return Results.NoContent();
    }

    /// <summary>
    /// Get all speakers for a specific conference
    /// </summary>
    private async Task<IResult> GetConferenceSpeakersAsync(
        string id, 
        ICosmosDbService<Speaker> speakersService)
    {
        var speakers = await speakersService.QueryItemsAsync(
            s => s.ConferenceIds.Contains(id),
            "Speaker");
        return Results.Ok(speakers);
    }

    /// <summary>
    /// Get all sessions for a specific conference
    /// </summary>
    private async Task<IResult> GetConferenceSessionsAsync(
        string id, 
        ICosmosDbService<Session> sessionsService)
    {
        var sessions = await sessionsService.QueryItemsAsync(
            s => s.ConferenceId == id,
            "Session");
            
        return Results.Ok(sessions);
    }

    /// <summary>
    /// Get all attendees for a specific conference
    /// </summary>
    private async Task<IResult> GetConferenceAttendeesAsync(
        string id, 
        ICosmosDbService<Attendee> attendeesService)
    {
        var attendees = await attendeesService.QueryItemsAsync(
            a => a.ConferenceRegistrations.ContainsKey(id),
            "Attendee");
        return Results.Ok(attendees);
    }

    /// <summary>
    /// Get all venues for a specific conference
    /// </summary>
    private async Task<IResult> GetConferenceVenuesAsync(
        string id, 
        ICosmosDbService<Venue> venuesService)
    {
        var venues = await venuesService.QueryItemsAsync(
            v => v.ConferenceIds.Contains(id),
            "Venue");
        return Results.Ok(venues);
    }
}
