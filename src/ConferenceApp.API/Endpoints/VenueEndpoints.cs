using ConferenceApp.Shared.Models;
using ConferenceApp.API.Services;
using FluentValidation;

namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Venue endpoints
/// </summary>
public class VenueEndpoints : IEndpoints
{    /// <summary>
    /// Maps venue endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api/venues")
            .WithTags("Venues")
            .WithOpenApi();

        // Get all venues
        group.MapGet("/", GetAllVenuesAsync)
            .WithName("GetVenues")
            .WithDescription("Get all venues")
            .Produces<IEnumerable<Venue>>(StatusCodes.Status200OK);
            
        // Get venues by conference
        group.MapGet("/conference/{conferenceId}", GetVenuesByConferenceAsync)
            .WithName("GetVenuesByConference")
            .WithDescription("Get venues for a specific conference")
            .Produces<IEnumerable<Venue>>(StatusCodes.Status200OK);

        // Get venue by ID
        group.MapGet("/{id}", GetVenueByIdAsync)
            .WithName("GetVenueById")
            .WithDescription("Get a venue by ID")
            .Produces<Venue>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Create new venue
        group.MapPost("/", CreateVenueAsync)
            .WithName("CreateVenue")
            .WithDescription("Create a new venue")
            .Produces<Venue>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update venue
        group.MapPut("/{id}", UpdateVenueAsync)
            .WithName("UpdateVenue")
            .WithDescription("Update an existing venue")
            .Produces<Venue>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete venue
        group.MapDelete("/{id}", DeleteVenueAsync)
            .WithName("DeleteVenue")
            .WithDescription("Delete a venue")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }    /// <summary>
    /// Get all venues
    /// </summary>
    private async Task<IResult> GetAllVenuesAsync(ICosmosDbService<Venue> cosmosDbService)
    {
        var venues = await cosmosDbService.GetItemsAsync("Venue");
        return Results.Ok(venues);
    }
      /// <summary>
    /// Get venues by conference ID
    /// </summary>
    private async Task<IResult> GetVenuesByConferenceAsync(
        string conferenceId, 
        ICosmosDbService<Venue> cosmosDbService)
    {
        var venues = await cosmosDbService.QueryItemsAsync(
            v => v.ConferenceIds.Contains(conferenceId),
            "Venue");
            
        return Results.Ok(venues);
    }

    /// <summary>
    /// Get venue by ID
    /// </summary>
    private async Task<IResult> GetVenueByIdAsync(string id, ICosmosDbService<Venue> cosmosDbService)
    {
        var venue = await cosmosDbService.GetItemAsync(id, "Venue");

        if (venue == null)
            return Results.NotFound();

        return Results.Ok(venue);
    }    /// <summary>
    /// Create a new venue
    /// </summary>
    private async Task<IResult> CreateVenueAsync(
        Venue venue, 
        ICosmosDbService<Venue> cosmosDbService,
        IValidator<Venue> validator)
    {
        // Check for existing venues with the same name and address
        var existingVenues = await cosmosDbService.QueryItemsAsync(
            v => v.Name == venue.Name && v.Address == venue.Address && v.City == venue.City,
            "Venue");
            
        if (existingVenues.Any())
        {
            // If venue exists, we can add the conference ID to its list of conferences
            var existingVenue = existingVenues.First();
            string conferenceId = venue.ConferenceIds.FirstOrDefault();
            
            if (!string.IsNullOrEmpty(conferenceId) && !existingVenue.ConferenceIds.Contains(conferenceId))
            {
                existingVenue.ConferenceIds.Add(conferenceId);
                await cosmosDbService.UpdateItemAsync(existingVenue.Id, existingVenue);
            }
            
            return Results.Ok(existingVenue);
        }
            
        var validationResult = await validator.ValidateAsync(venue);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.AddItemAsync(venue);
        return Results.Created($"/api/venues/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing venue
    /// </summary>
    private async Task<IResult> UpdateVenueAsync(
        string id, 
        Venue venue, 
        ICosmosDbService<Venue> cosmosDbService,
        IValidator<Venue> validator)
    {
        var existingVenue = await cosmosDbService.GetItemAsync(id, "Venue");
        
        if (existingVenue == null)
            return Results.NotFound();

        // Ensure ID and partition key match
        venue.Id = id;
        venue.PartitionKey = "Venue";
        
        var validationResult = await validator.ValidateAsync(venue);
        
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await cosmosDbService.UpdateItemAsync(id, venue);
        return Results.Ok(result);
    }

    /// <summary>
    /// Delete a venue
    /// </summary>
    private async Task<IResult> DeleteVenueAsync(string id, ICosmosDbService<Venue> cosmosDbService)
    {
        var venue = await cosmosDbService.GetItemAsync(id, "Venue");
        
        if (venue == null)
            return Results.NotFound();

        await cosmosDbService.DeleteItemAsync(id, "Venue");
        return Results.NoContent();
    }
}
