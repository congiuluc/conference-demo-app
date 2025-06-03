using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Speaker endpoints
/// </summary>
public class SpeakerEndpoints : IEndpoints
{    /// <summary>
    /// Maps speaker endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/speakers")
            .WithTags("Speakers")
            .WithOpenApi();

        // Get all speakers
        group.MapGet("/", GetAllSpeakersAsync)
            .WithName("GetSpeakers")
            .WithDescription("Get all speakers")
            .Produces<IEnumerable<Speaker>>(StatusCodes.Status200OK);
            
        // Get speakers by conference
        group.MapGet("/conference/{conferenceId}", GetSpeakersByConferenceAsync)
            .WithName("GetSpeakersByConference")
            .WithDescription("Get speakers for a specific conference")
            .Produces<IEnumerable<Speaker>>(StatusCodes.Status200OK);

        // Get speaker by ID
        group.MapGet("/{id}", GetSpeakerByIdAsync)
            .WithName("GetSpeakerById")
            .WithDescription("Get a speaker by ID")
            .Produces<Speaker>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Create new speaker
        group.MapPost("/", CreateSpeakerAsync)
            .WithName("CreateSpeaker")
            .WithDescription("Create a new speaker")
            .Produces<Speaker>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update speaker
        group.MapPut("/{id}", UpdateSpeakerAsync)
            .WithName("UpdateSpeaker")
            .WithDescription("Update an existing speaker")
            .Produces<Speaker>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete speaker
        group.MapDelete("/{id}", DeleteSpeakerAsync)
            .WithName("DeleteSpeaker")
            .WithDescription("Delete a speaker")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }    /// <summary>
    /// Get all speakers
    /// </summary>
    private async Task<IResult> GetAllSpeakersAsync(ICosmosDbService<Speaker> cosmosDbService)
    {
        var speakers = await cosmosDbService.GetItemsAsync("Speaker");
        return Results.Ok(speakers);
    }
      /// <summary>
    /// Get speakers by conference ID
    /// </summary>
    private async Task<IResult> GetSpeakersByConferenceAsync(
        string conferenceId, 
        ICosmosDbService<Speaker> cosmosDbService)
    {
        var speakers = await cosmosDbService.QueryItemsAsync(
            s => s.ConferenceIds.Contains(conferenceId),
            "Speaker");
            
        return Results.Ok(speakers);
    }

    /// <summary>
    /// Get speaker by ID
    /// </summary>
    private async Task<IResult> GetSpeakerByIdAsync(string id, ICosmosDbService<Speaker> cosmosDbService)
    {
        var speaker = await cosmosDbService.GetItemAsync(id, "Speaker");

        if (speaker == null)
            return Results.NotFound();

        return Results.Ok(speaker);
    }    /// <summary>
    /// Create a new speaker
    /// </summary>
    private async Task<IResult> CreateSpeakerAsync(
        Speaker speaker, 
        ICosmosDbService<Speaker> cosmosDbService,
        IValidator<Speaker> validator)
    {
        // First check if a speaker with the same email already exists
        var existingSpeakers = await cosmosDbService.QueryItemsAsync(
            s => s.Email == speaker.Email,
            "Speaker");
            
        if (existingSpeakers.Any())
        {
            // If speaker exists, we can add the conference ID to their list of conferences
            var existingSpeaker = existingSpeakers.First();
            string conferenceId = speaker.ConferenceIds.FirstOrDefault();
            
            if (!string.IsNullOrEmpty(conferenceId) && !existingSpeaker.ConferenceIds.Contains(conferenceId))
            {
                existingSpeaker.ConferenceIds.Add(conferenceId);
                await cosmosDbService.UpdateItemAsync(existingSpeaker.Id, existingSpeaker);
            }
            
            return Results.Ok(existingSpeaker);
        }
            
        var validationResult = await validator.ValidateAsync(speaker);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(speaker);
        return Results.Created($"/api/speakers/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing speaker
    /// </summary>
    private async Task<IResult> UpdateSpeakerAsync(
        string id, 
        Speaker speaker, 
        ICosmosDbService<Speaker> cosmosDbService,
        IValidator<Speaker> validator)
    {
        var existingSpeaker = await cosmosDbService.GetItemAsync(id, "Speaker");
        
        if (existingSpeaker == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        speaker.Id = id;
        speaker.PartitionKey = "Speaker";
        
        var validationResult = await validator.ValidateAsync(speaker);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, speaker);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete a speaker
    /// </summary>
    private async Task<IResult> DeleteSpeakerAsync(string id, ICosmosDbService<Speaker> cosmosDbService)
    {
        var speaker = await cosmosDbService.GetItemAsync(id, "Speaker");
        
        if (speaker == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "Speaker");
        return Results.NoContent();
    }
}
