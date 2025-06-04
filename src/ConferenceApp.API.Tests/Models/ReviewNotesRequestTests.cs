using ConferenceApp.API.Endpoints;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.API.Tests.Models;

public class ReviewNotesRequestTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var request = new ReviewNotesRequest();

        // Assert
        request.Notes.Should().Be(default!); // Will be null because of default!
        request.Status.Should().BeNull();
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var request = new ReviewNotesRequest();
        var notes = "Great session proposal with detailed agenda";
        var status = "Accepted";

        // Act
        request.Notes = notes;
        request.Status = status;

        // Assert
        request.Notes.Should().Be(notes);
        request.Status.Should().Be(status);
    }

    [Fact]
    public void Status_WhenSetToNull_ShouldBeNull()
    {
        // Arrange
        var request = new ReviewNotesRequest
        {
            Status = "Accepted"
        };

        // Act
        request.Status = null;

        // Assert
        request.Status.Should().BeNull();
    }

    [Fact]
    public void Status_WhenSetToEmpty_ShouldBeEmpty()
    {
        // Arrange
        var request = new ReviewNotesRequest();

        // Act
        request.Status = "";

        // Assert
        request.Status.Should().Be("");
    }
}