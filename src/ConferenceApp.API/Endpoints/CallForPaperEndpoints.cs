using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Call for Papers endpoints
/// </summary>
public class CallForPaperEndpoints : IEndpoints
{
    /// <summary>
    /// Maps call for papers endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/callforpapers")
            .WithTags("Call for Papers")
            .WithOpenApi();

        // Get all call for papers
        group.MapGet("/", GetAllCallForPapersAsync)
            .WithName("GetCallForPapers")
            .WithDescription("Get all call for papers")
            .Produces<IEnumerable<CallForPaper>>(StatusCodes.Status200OK);
            
        // Get call for papers by conference
        group.MapGet("/conference/{conferenceId}", GetCallForPapersByConferenceAsync)
            .WithName("GetCallForPapersByConference")
            .WithDescription("Get call for papers for a specific conference")
            .Produces<IEnumerable<CallForPaper>>(StatusCodes.Status200OK);

        // Get call for paper by ID
        group.MapGet("/{id}", GetCallForPaperByIdAsync)
            .WithName("GetCallForPaperById")
            .WithDescription("Get a call for paper by ID")
            .Produces<CallForPaper>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Create new call for paper
        group.MapPost("/", CreateCallForPaperAsync)
            .WithName("CreateCallForPaper")
            .WithDescription("Create a new call for paper")
            .Produces<CallForPaper>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update call for paper
        group.MapPut("/{id}", UpdateCallForPaperAsync)
            .WithName("UpdateCallForPaper")
            .WithDescription("Update an existing call for paper")
            .Produces<CallForPaper>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete call for paper
        group.MapDelete("/{id}", DeleteCallForPaperAsync)
            .WithName("DeleteCallForPaper")
            .WithDescription("Delete a call for paper")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
            
        // Close call for papers
        group.MapPost("/{id}/close", CloseCallForPaperAsync)
            .WithName("CloseCallForPaper")
            .WithDescription("Close a call for paper")
            .Produces<CallForPaper>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            
        // Submit a session proposal to a call for papers
        group.MapPost("/{id}/submit", SubmitSessionProposalAsync)
            .WithName("SubmitSessionProposal")
            .WithDescription("Submit a session proposal to a call for papers")
            .Produces<Session>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }    
    
    /// <summary>
    /// Get all call for papers
    /// </summary>
    private async Task<IResult> GetAllCallForPapersAsync(ICosmosDbService<CallForPaper> cosmosDbService)
    {
        var callForPapers = await cosmosDbService.GetItemsAsync("CallForPaper");
        return Results.Ok(callForPapers);
    }
    
    /// <summary>
    /// Get call for papers by conference ID
    /// </summary>
    private async Task<IResult> GetCallForPapersByConferenceAsync(
        string conferenceId, 
        ICosmosDbService<CallForPaper> cosmosDbService)
    {
        var callForPapers = await cosmosDbService.QueryItemsAsync(
            c => c.ConferenceId == conferenceId,
            "CallForPaper");
            
        return Results.Ok(callForPapers);
    }

    /// <summary>
    /// Get call for paper by ID
    /// </summary>
    private async Task<IResult> GetCallForPaperByIdAsync(string id, ICosmosDbService<CallForPaper> cosmosDbService)
    {
        var callForPaper = await cosmosDbService.GetItemAsync(id, "CallForPaper");

        if (callForPaper == null)
            return Results.NotFound();

        return Results.Ok(callForPaper);
    }

    /// <summary>
    /// Create a new call for paper
    /// </summary>
    private async Task<IResult> CreateCallForPaperAsync(
        CallForPaper callForPaper, 
        ICosmosDbService<CallForPaper> cosmosDbService,
        ICosmosDbService<Conference> conferenceService,
        IValidator<CallForPaper> validator)
    {
        // First check if the conference exists
        var conference = await conferenceService.GetItemAsync(callForPaper.ConferenceId, "Conference");
        if (conference == null)
            return Results.BadRequest($"Conference with ID {callForPaper.ConferenceId} does not exist");
            
        var validationResult = await validator.ValidateAsync(callForPaper);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(callForPaper);
        return Results.Created($"/api/callforpapers/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing call for paper
    /// </summary>
    private async Task<IResult> UpdateCallForPaperAsync(
        string id, 
        CallForPaper callForPaper, 
        ICosmosDbService<CallForPaper> cosmosDbService,
        IValidator<CallForPaper> validator)
    {
        var existingCallForPaper = await cosmosDbService.GetItemAsync(id, "CallForPaper");
        
        if (existingCallForPaper == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        callForPaper.Id = id;
        callForPaper.PartitionKey = "CallForPaper";
        
        var validationResult = await validator.ValidateAsync(callForPaper);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, callForPaper);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete a call for paper
    /// </summary>
    private async Task<IResult> DeleteCallForPaperAsync(string id, ICosmosDbService<CallForPaper> cosmosDbService)
    {
        var callForPaper = await cosmosDbService.GetItemAsync(id, "CallForPaper");
        
        if (callForPaper == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "CallForPaper");
        return Results.NoContent();
    }
    
    /// <summary>
    /// Close a call for paper
    /// </summary>
    private async Task<IResult> CloseCallForPaperAsync(string id, ICosmosDbService<CallForPaper> cosmosDbService)
    {
        var callForPaper = await cosmosDbService.GetItemAsync(id, "CallForPaper");
        
        if (callForPaper == null)
            return Results.NotFound();

        callForPaper.IsOpen = false;
        var result = await cosmosDbService.UpdateItemAsync(id, callForPaper);
        return Results.Ok(result);
    }
    
    /// <summary>
    /// Submit a session proposal to a call for papers
    /// </summary>
    private async Task<IResult> SubmitSessionProposalAsync(
        string id, 
        Session session,
        ICosmosDbService<CallForPaper> callForPaperService,
        ICosmosDbService<Session> sessionService,
        ICosmosDbService<Speaker> speakerService,
        IValidator<Session> validator)
    {
        var callForPaper = await callForPaperService.GetItemAsync(id, "CallForPaper");
        
        if (callForPaper == null)
            return Results.NotFound("Call for papers not found");
            
        if (!callForPaper.IsOpen)
            return Results.BadRequest("Call for papers is closed");
            
        // Set call for paper reference and conference ID
        session.CallForPaperId = id;
        session.ConferenceId = callForPaper.ConferenceId;
        session.Status = SessionStatus.Proposed;
        
        // Verify that session type is allowed
        if (!callForPaper.SessionTypes.Contains(session.SessionType))
            return Results.BadRequest($"Session type '{session.SessionType}' is not allowed for this call for papers");
            
        // Verify that all speakers exist
        foreach (var speakerId in session.SpeakerIds)
        {
            var speaker = await speakerService.GetItemAsync(speakerId, "Speaker");
            if (speaker == null)
                return Results.BadRequest($"Speaker with ID {speakerId} does not exist");
                
            // Add conference ID to speaker's conferences if not already there
            if (!speaker.ConferenceIds.Contains(callForPaper.ConferenceId))
            {
                speaker.ConferenceIds.Add(callForPaper.ConferenceId);
                await speakerService.UpdateItemAsync(speakerId, speaker);
            }
        }
            
        var validationResult = await validator.ValidateAsync(session);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await sessionService.AddItemAsync(session);
        return Results.Created($"/api/sessions/{result.Id}", result);
    }
}
