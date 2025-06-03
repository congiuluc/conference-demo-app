using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Agenda endpoints
/// </summary>
public class AgendaEndpoints : IEndpoints
{
    /// <summary>
    /// Maps agenda endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/agenda")
            .WithTags("Agenda")
            .WithOpenApi();

        // Get all agenda days
        group.MapGet("/", GetAllAgendaDaysAsync)
            .WithName("GetAllAgendaDays")
            .WithDescription("Get all agenda days")
            .Produces<IEnumerable<AgendaDay>>(StatusCodes.Status200OK);
            
        // Get agenda days by conference
        group.MapGet("/conference/{conferenceId}", GetAgendaDaysByConferenceAsync)
            .WithName("GetAgendaDaysByConference")
            .WithDescription("Get agenda days for a specific conference")
            .Produces<IEnumerable<AgendaDay>>(StatusCodes.Status200OK);
            
        // Get agenda day by ID
        group.MapGet("/{id}", GetAgendaDayByIdAsync)
            .WithName("GetAgendaDayById")
            .WithDescription("Get an agenda day by ID")
            .Produces<AgendaDay>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            
        // Get agenda by conference and date
        group.MapGet("/conference/{conferenceId}/date/{date}", GetAgendaByDateAsync)
            .WithName("GetAgendaByDate")
            .WithDescription("Get agenda for a specific conference and date")
            .Produces<AgendaDay>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Create new agenda day
        group.MapPost("/", CreateAgendaDayAsync)
            .WithName("CreateAgendaDay")
            .WithDescription("Create a new agenda day")
            .Produces<AgendaDay>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update agenda day
        group.MapPut("/{id}", UpdateAgendaDayAsync)
            .WithName("UpdateAgendaDay")
            .WithDescription("Update an existing agenda day")
            .Produces<AgendaDay>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete agenda day
        group.MapDelete("/{id}", DeleteAgendaDayAsync)
            .WithName("DeleteAgendaDay")
            .WithDescription("Delete an agenda day")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
              // Add session to agenda
        group.MapPost("/{id}/sessions/{sessionId}", AddSessionToAgendaAsync)
            .WithName("AddSessionToAgenda")
            .WithDescription("Add a session to an agenda day with specific venue and room")
            .Produces<AgendaDay>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
            
        // Remove session from agenda
        group.MapDelete("/{id}/sessions/{sessionId}", RemoveSessionFromAgendaAsync)
            .WithName("RemoveSessionFromAgenda")
            .WithDescription("Remove a session from an agenda day")
            .Produces<AgendaDay>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }    
    
    /// <summary>
    /// Get all agenda days
    /// </summary>
    private async Task<IResult> GetAllAgendaDaysAsync(ICosmosDbService<AgendaDay> cosmosDbService)
    {
        var agendaDays = await cosmosDbService.GetItemsAsync("AgendaDay");
        return Results.Ok(agendaDays);
    }
    
    /// <summary>
    /// Get agenda days by conference ID
    /// </summary>
    private async Task<IResult> GetAgendaDaysByConferenceAsync(
        string conferenceId, 
        ICosmosDbService<AgendaDay> cosmosDbService)
    {
        var agendaDays = await cosmosDbService.QueryItemsAsync(
            a => a.ConferenceId == conferenceId,
            "AgendaDay");
            
        return Results.Ok(agendaDays);
    }
    
    /// <summary>
    /// Get agenda day by ID
    /// </summary>
    private async Task<IResult> GetAgendaDayByIdAsync(string id, ICosmosDbService<AgendaDay> cosmosDbService)
    {
        var agendaDay = await cosmosDbService.GetItemAsync(id, "AgendaDay");

        if (agendaDay == null)
            return Results.NotFound();

        return Results.Ok(agendaDay);
    }
    
    /// <summary>
    /// Get agenda by conference ID and date
    /// </summary>
    private async Task<IResult> GetAgendaByDateAsync(
        string conferenceId, 
        DateTime date, 
        ICosmosDbService<AgendaDay> cosmosDbService)
    {
        var agendaDays = await cosmosDbService.QueryItemsAsync(
            a => a.ConferenceId == conferenceId && a.Date.Date == date.Date,
            "AgendaDay");
            
        var agendaDay = agendaDays.FirstOrDefault();
        
        if (agendaDay == null)
            return Results.NotFound();
            
        return Results.Ok(agendaDay);
    }

    /// <summary>
    /// Create a new agenda day
    /// </summary>
    private async Task<IResult> CreateAgendaDayAsync(
        AgendaDay agendaDay, 
        ICosmosDbService<AgendaDay> cosmosDbService,
        ICosmosDbService<Conference> conferenceService,
        IValidator<AgendaDay> validator)
    {
        // First check if the conference exists
        var conference = await conferenceService.GetItemAsync(agendaDay.ConferenceId, "Conference");
        if (conference == null)
            return Results.BadRequest($"Conference with ID {agendaDay.ConferenceId} does not exist");
            
        // Check if an agenda day already exists for this conference and date
        var existingAgendaDays = await cosmosDbService.QueryItemsAsync(
            a => a.ConferenceId == agendaDay.ConferenceId && a.Date.Date == agendaDay.Date.Date,
            "AgendaDay");
            
        if (existingAgendaDays.Any())
            return Results.BadRequest($"An agenda day already exists for conference {agendaDay.ConferenceId} on {agendaDay.Date.ToShortDateString()}");
            
        var validationResult = await validator.ValidateAsync(agendaDay);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(agendaDay);
        return Results.Created($"/api/agenda/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing agenda day
    /// </summary>
    private async Task<IResult> UpdateAgendaDayAsync(
        string id, 
        AgendaDay agendaDay, 
        ICosmosDbService<AgendaDay> cosmosDbService,
        IValidator<AgendaDay> validator)
    {
        var existingAgendaDay = await cosmosDbService.GetItemAsync(id, "AgendaDay");
        
        if (existingAgendaDay == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        agendaDay.Id = id;
        agendaDay.PartitionKey = "AgendaDay";
        
        var validationResult = await validator.ValidateAsync(agendaDay);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, agendaDay);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete an agenda day
    /// </summary>
    private async Task<IResult> DeleteAgendaDayAsync(string id, ICosmosDbService<AgendaDay> cosmosDbService)
    {
        var agendaDay = await cosmosDbService.GetItemAsync(id, "AgendaDay");
        
        if (agendaDay == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "AgendaDay");
        return Results.NoContent();
    }
      /// <summary>
    /// Add a session to an agenda day
    /// </summary>
    private async Task<IResult> AddSessionToAgendaAsync(
        string id, 
        string sessionId,
        string track,
        [FromQuery] string venueId,
        [FromQuery] string room,
        ICosmosDbService<AgendaDay> agendaDayService,
        ICosmosDbService<Session> sessionService)
    {
        var agendaDay = await agendaDayService.GetItemAsync(id, "AgendaDay");
        
        if (agendaDay == null)
            return Results.NotFound("Agenda day not found");
            
        var session = await sessionService.GetItemAsync(sessionId, "Session");
        
        if (session == null)
            return Results.NotFound("Session not found");
            
        // Make sure session is for the same conference
        if (session.ConferenceId != agendaDay.ConferenceId)
            return Results.BadRequest("Session is for a different conference");
            
        // Make sure session date matches agenda day date
        if (session.StartTime.Date != agendaDay.Date.Date)
            return Results.BadRequest("Session date does not match agenda day date");
            
        // Make sure session is accepted or scheduled
        if (session.Status != SessionStatus.Accepted && session.Status != SessionStatus.Scheduled)
            return Results.BadRequest("Only accepted or scheduled sessions can be added to the agenda");
            
        // Create time slot for track if it doesn't exist
        if (!agendaDay.TimeSlotsByTrack.ContainsKey(track))
        {
            agendaDay.TimeSlotsByTrack[track] = new List<AgendaTimeSlot>();
        }
          // Check for time conflicts in the same venue/room combination
        var conflictingTimeSlots = agendaDay.TimeSlotsByTrack.Values
            .SelectMany(slots => slots)
            .Where(ts => 
                ts.VenueId == venueId && ts.Room == room && // Same venue and room
                ((ts.StartTime < session.EndTime && ts.EndTime > session.StartTime) || // Overlapping times
                ts.SessionId == sessionId) // Same session already added
            ).ToList();
          if (conflictingTimeSlots.Any())
            return Results.BadRequest($"Time conflict detected in venue {venueId}, room {room} or session already added to the agenda");
              // Add session to agenda
        var timeSlot = new AgendaTimeSlot
        {
            SessionId = sessionId,
            Title = session.Title,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            SlotType = session.SessionType,
            // These need to be provided when adding to agenda since they're no longer in the Session model
            VenueId = venueId, // This should be passed as a parameter
            Room = room // This should be passed as a parameter
        };
        
        agendaDay.TimeSlotsByTrack[track].Add(timeSlot);
        
        // Update session status to scheduled
        session.Status = SessionStatus.Scheduled;
        await sessionService.UpdateItemAsync(sessionId, session);
        
        // Sort time slots by start time
        agendaDay.TimeSlotsByTrack[track] = agendaDay.TimeSlotsByTrack[track]
            .OrderBy(ts => ts.StartTime)
            .ToList();
            
        var result = await agendaDayService.UpdateItemAsync(id, agendaDay);
        return Results.Ok(result);
    }
    
    /// <summary>
    /// Remove a session from an agenda day
    /// </summary>
    private async Task<IResult> RemoveSessionFromAgendaAsync(
        string id, 
        string sessionId,
        ICosmosDbService<AgendaDay> agendaDayService,
        ICosmosDbService<Session> sessionService)
    {
        var agendaDay = await agendaDayService.GetItemAsync(id, "AgendaDay");
        
        if (agendaDay == null)
            return Results.NotFound("Agenda day not found");
            
        bool sessionFound = false;
        
        // Find and remove the session from all tracks
        foreach (var track in agendaDay.TimeSlotsByTrack.Keys.ToList())
        {
            var timeSlot = agendaDay.TimeSlotsByTrack[track].FirstOrDefault(ts => ts.SessionId == sessionId);
            
            if (timeSlot != null)
            {
                agendaDay.TimeSlotsByTrack[track].Remove(timeSlot);
                sessionFound = true;
                
                // Remove empty tracks
                if (agendaDay.TimeSlotsByTrack[track].Count == 0)
                {
                    agendaDay.TimeSlotsByTrack.Remove(track);
                }
            }
        }
        
        if (!sessionFound)
            return Results.NotFound("Session not found in agenda");
            
        // Update session status back to accepted
        var session = await sessionService.GetItemAsync(sessionId, "Session");
        
        if (session != null && session.Status == SessionStatus.Scheduled)
        {
            session.Status = SessionStatus.Accepted;
            await sessionService.UpdateItemAsync(sessionId, session);
        }
        
        var result = await agendaDayService.UpdateItemAsync(id, agendaDay);
        return Results.Ok(result);
    }
}
