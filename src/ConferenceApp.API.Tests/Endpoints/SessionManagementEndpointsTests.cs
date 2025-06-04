using ConferenceApp.API.Endpoints;
using ConferenceApp.API.Services;
using ConferenceApp.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.Reflection;
using Xunit;

namespace ConferenceApp.API.Tests.Endpoints;

public class SessionManagementEndpointsTests
{
    private readonly Mock<ICosmosDbService<Session>> _mockCosmosDbService;
    private readonly SessionManagementEndpoints _endpoints;

    public SessionManagementEndpointsTests()
    {
        _mockCosmosDbService = new Mock<ICosmosDbService<Session>>();
        _endpoints = new SessionManagementEndpoints();
    }

    [Fact]
    public async Task UpdateSessionStatusAsync_WithValidStatus_ShouldUpdateSession()
    {
        // Arrange
        var sessionId = "session123";
        var status = "Accepted";
        var session = new Session
        {
            Id = sessionId,
            Title = "Test Session",
            Status = SessionStatus.Proposed
        };

        _mockCosmosDbService
            .Setup(s => s.GetItemAsync(sessionId, "Session"))
            .ReturnsAsync(session);

        _mockCosmosDbService
            .Setup(s => s.UpdateItemAsync(sessionId, It.IsAny<Session>()))
            .ReturnsAsync(session);

        // Act
        var result = await InvokeUpdateSessionStatusAsync(sessionId, status, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<Ok<Session>>();
        session.Status.Should().Be(SessionStatus.Accepted);
        _mockCosmosDbService.Verify(s => s.UpdateItemAsync(sessionId, session), Times.Once);
    }

    [Fact]
    public async Task UpdateSessionStatusAsync_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var sessionId = "session123";
        var invalidStatus = "InvalidStatus";

        // Act
        var result = await InvokeUpdateSessionStatusAsync(sessionId, invalidStatus, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<BadRequest<string>>();
        var badRequestResult = result as BadRequest<string>;
        badRequestResult!.Value.Should().Contain("Invalid status");
    }

    [Fact]
    public async Task UpdateSessionStatusAsync_WithNonExistentSession_ShouldReturnNotFound()
    {
        // Arrange
        var sessionId = "nonexistent";
        var status = "Accepted";

        _mockCosmosDbService
            .Setup(s => s.GetItemAsync(sessionId, "Session"))
            .ReturnsAsync((Session?)null);

        // Act
        var result = await InvokeUpdateSessionStatusAsync(sessionId, status, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task AddReviewNotesToSessionAsync_WithValidRequest_ShouldUpdateSession()
    {
        // Arrange
        var sessionId = "session123";
        var request = new ReviewNotesRequest
        {
            Notes = "Great session proposal",
            Status = "Accepted"
        };
        var session = new Session
        {
            Id = sessionId,
            Title = "Test Session",
            Status = SessionStatus.Proposed
        };

        _mockCosmosDbService
            .Setup(s => s.GetItemAsync(sessionId, "Session"))
            .ReturnsAsync(session);

        _mockCosmosDbService
            .Setup(s => s.UpdateItemAsync(sessionId, It.IsAny<Session>()))
            .ReturnsAsync(session);

        // Act
        var result = await InvokeAddReviewNotesToSessionAsync(sessionId, request, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<Ok<Session>>();
        session.ReviewNotes.Should().Be("Great session proposal");
        session.Status.Should().Be(SessionStatus.Accepted);
        _mockCosmosDbService.Verify(s => s.UpdateItemAsync(sessionId, session), Times.Once);
    }

    [Fact]
    public async Task AddReviewNotesToSessionAsync_WithNotesOnly_ShouldUpdateNotesButNotStatus()
    {
        // Arrange
        var sessionId = "session123";
        var request = new ReviewNotesRequest
        {
            Notes = "Good presentation structure",
            Status = null
        };
        var session = new Session
        {
            Id = sessionId,
            Title = "Test Session",
            Status = SessionStatus.UnderReview
        };

        _mockCosmosDbService
            .Setup(s => s.GetItemAsync(sessionId, "Session"))
            .ReturnsAsync(session);

        _mockCosmosDbService
            .Setup(s => s.UpdateItemAsync(sessionId, It.IsAny<Session>()))
            .ReturnsAsync(session);

        // Act
        var result = await InvokeAddReviewNotesToSessionAsync(sessionId, request, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<Ok<Session>>();
        session.ReviewNotes.Should().Be("Good presentation structure");
        session.Status.Should().Be(SessionStatus.UnderReview); // Should remain unchanged
    }

    [Fact]
    public async Task AddReviewNotesToSessionAsync_WithInvalidStatus_ShouldUpdateNotesButIgnoreStatus()
    {
        // Arrange
        var sessionId = "session123";
        var request = new ReviewNotesRequest
        {
            Notes = "Good session",
            Status = "InvalidStatus"
        };
        var session = new Session
        {
            Id = sessionId,
            Title = "Test Session",
            Status = SessionStatus.Proposed
        };

        _mockCosmosDbService
            .Setup(s => s.GetItemAsync(sessionId, "Session"))
            .ReturnsAsync(session);

        _mockCosmosDbService
            .Setup(s => s.UpdateItemAsync(sessionId, It.IsAny<Session>()))
            .ReturnsAsync(session);

        // Act
        var result = await InvokeAddReviewNotesToSessionAsync(sessionId, request, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<Ok<Session>>();
        session.ReviewNotes.Should().Be("Good session");
        session.Status.Should().Be(SessionStatus.Proposed); // Should remain unchanged
    }

    [Fact]
    public async Task AddReviewNotesToSessionAsync_WithNonExistentSession_ShouldReturnNotFound()
    {
        // Arrange
        var sessionId = "nonexistent";
        var request = new ReviewNotesRequest
        {
            Notes = "Test notes",
            Status = "Accepted"
        };

        _mockCosmosDbService
            .Setup(s => s.GetItemAsync(sessionId, "Session"))
            .ReturnsAsync((Session?)null);

        // Act
        var result = await InvokeAddReviewNotesToSessionAsync(sessionId, request, _mockCosmosDbService.Object);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    // Helper methods to invoke private methods using reflection
    private async Task<IResult> InvokeUpdateSessionStatusAsync(string id, string status, ICosmosDbService<Session> cosmosDbService)
    {
        var method = typeof(SessionManagementEndpoints).GetMethod("UpdateSessionStatusAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task<IResult>)method!.Invoke(_endpoints, new object[] { id, status, cosmosDbService })!;
        return await task;
    }

    private async Task<IResult> InvokeAddReviewNotesToSessionAsync(string id, ReviewNotesRequest request, ICosmosDbService<Session> cosmosDbService)
    {
        var method = typeof(SessionManagementEndpoints).GetMethod("AddReviewNotesToSessionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task<IResult>)method!.Invoke(_endpoints, new object[] { id, request, cosmosDbService })!;
        return await task;
    }
}