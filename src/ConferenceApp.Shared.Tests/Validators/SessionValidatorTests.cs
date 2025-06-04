using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ConferenceApp.Shared.Tests.Validators;

public class SessionValidatorTests
{
    private readonly SessionValidator _validator;

    public SessionValidatorTests()
    {
        _validator = new SessionValidator();
    }

    [Fact]
    public void Validate_WithValidSession_ShouldPassValidation()
    {
        // Arrange
        var session = new Session
        {
            Title = "Valid Session Title",
            Description = "Valid session description",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Track = "Backend",
            Level = "Intermediate",
            ConferenceId = "conf123",
            SessionType = "Talk",
            SpeakerIds = { "speaker1" }
        };

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Title_WhenNullOrEmpty_ShouldHaveValidationError(string title)
    {
        // Arrange
        var session = CreateValidSession();
        session.Title = title;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Title_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.Title = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Description_WhenNullOrEmpty_ShouldHaveValidationError(string description)
    {
        // Arrange
        var session = CreateValidSession();
        session.Description = description;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.Description = new string('a', 2001); // Exceeds 2000 character limit

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 2000 characters");
    }

    [Fact]
    public void StartTime_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartTime = default;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartTime)
            .WithErrorMessage("Start time is required");
    }

    [Fact]
    public void StartTime_WhenAfterEndTime_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartTime = DateTime.UtcNow.AddHours(2);
        session.EndTime = DateTime.UtcNow.AddHours(1);

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartTime)
            .WithErrorMessage("Start time must be before end time");
    }

    [Fact]
    public void EndTime_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.EndTime = default;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("End time is required");
    }

    [Fact]
    public void EndTime_WhenBeforeStartTime_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartTime = DateTime.UtcNow.AddHours(2);
        session.EndTime = DateTime.UtcNow.AddHours(1);

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("End time must be after start time");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Track_WhenNullOrEmpty_ShouldHaveValidationError(string track)
    {
        // Arrange
        var session = CreateValidSession();
        session.Track = track;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Track)
            .WithErrorMessage("Track is required");
    }

    [Fact]
    public void Track_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.Track = new string('a', 101); // Exceeds 100 character limit

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Track)
            .WithErrorMessage("Track cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Level_WhenNullOrEmpty_ShouldHaveValidationError(string level)
    {
        // Arrange
        var session = CreateValidSession();
        session.Level = level;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level)
            .WithErrorMessage("Level is required");
    }

    [Theory]
    [InlineData("Beginner")]
    [InlineData("Intermediate")]
    [InlineData("Advanced")]
    [InlineData("beginner")]
    [InlineData("INTERMEDIATE")]
    [InlineData("advanced")]
    public void Level_WithValidValues_ShouldPassValidation(string level)
    {
        // Arrange
        var session = CreateValidSession();
        session.Level = level;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Level);
    }

    [Theory]
    [InlineData("Expert")]
    [InlineData("Novice")]
    [InlineData("Invalid")]
    public void Level_WithInvalidValues_ShouldHaveValidationError(string level)
    {
        // Arrange
        var session = CreateValidSession();
        session.Level = level;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level)
            .WithErrorMessage("Level must be Beginner, Intermediate, or Advanced");
    }

    [Fact]
    public void SpeakerIds_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.SpeakerIds.Clear();

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SpeakerIds)
            .WithErrorMessage("At least one speaker must be assigned");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ConferenceId_WhenNullOrEmpty_ShouldHaveValidationError(string conferenceId)
    {
        // Arrange
        var session = CreateValidSession();
        session.ConferenceId = conferenceId;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConferenceId)
            .WithErrorMessage("Conference ID is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void SessionType_WhenNullOrEmpty_ShouldHaveValidationError(string sessionType)
    {
        // Arrange
        var session = CreateValidSession();
        session.SessionType = sessionType;

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SessionType)
            .WithErrorMessage("Session type is required");
    }

    [Fact]
    public void SessionType_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var session = CreateValidSession();
        session.SessionType = new string('a', 51); // Exceeds 50 character limit

        // Act
        var result = _validator.TestValidate(session);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SessionType)
            .WithErrorMessage("Session type cannot exceed 50 characters");
    }

    private Session CreateValidSession()
    {
        return new Session
        {
            Title = "Valid Session Title",
            Description = "Valid session description",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Track = "Backend",
            Level = "Intermediate",
            ConferenceId = "conf123",
            SessionType = "Talk",
            SpeakerIds = { "speaker1" }
        };
    }
}