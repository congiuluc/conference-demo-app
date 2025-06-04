using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class SessionTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var session = new Session();

        // Assert
        session.PartitionKey.Should().Be("Session");
        session.Status.Should().Be(SessionStatus.Proposed);
        session.SpeakerIds.Should().NotBeNull().And.BeEmpty();
        session.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Duration_ShouldCalculateCorrectly()
    {
        // Arrange
        var session = new Session
        {
            StartTime = new DateTime(2024, 1, 1, 10, 0, 0),
            EndTime = new DateTime(2024, 1, 1, 11, 30, 0)
        };

        // Act & Assert
        session.Duration.Should().Be(90); // 90 minutes
    }

    [Fact]
    public void Duration_WhenSet_ShouldUpdateEndTime()
    {
        // Arrange
        var session = new Session
        {
            StartTime = new DateTime(2024, 1, 1, 10, 0, 0)
        };

        // Act
        session.Duration = 60;

        // Assert
        session.EndTime.Should().Be(new DateTime(2024, 1, 1, 11, 0, 0));
    }

    [Fact]
    public void SpeakerId_WhenGet_ShouldReturnFirstSpeakerId()
    {
        // Arrange
        var session = new Session();
        session.SpeakerIds.Add("speaker1");
        session.SpeakerIds.Add("speaker2");

        // Act & Assert
        session.SpeakerId.Should().Be("speaker1");
    }

    [Fact]
    public void SpeakerId_WhenSetToNull_ShouldClearSpeakerIds()
    {
        // Arrange
        var session = new Session();
        session.SpeakerIds.Add("speaker1");
        session.SpeakerIds.Add("speaker2");

        // Act
        session.SpeakerId = null;

        // Assert
        session.SpeakerIds.Should().BeEmpty();
    }

    [Fact]
    public void SpeakerId_WhenSetToEmptyString_ShouldClearSpeakerIds()
    {
        // Arrange
        var session = new Session();
        session.SpeakerIds.Add("speaker1");

        // Act
        session.SpeakerId = "";

        // Assert
        session.SpeakerIds.Should().BeEmpty();
    }

    [Fact]
    public void SpeakerId_WhenSetToValue_ShouldReplaceAllSpeakerIds()
    {
        // Arrange
        var session = new Session();
        session.SpeakerIds.Add("speaker1");
        session.SpeakerIds.Add("speaker2");

        // Act
        session.SpeakerId = "speaker3";

        // Assert
        session.SpeakerIds.Should().HaveCount(1);
        session.SpeakerIds.Should().Contain("speaker3");
    }

    [Fact]
    public void ScheduledStartTime_ShouldReturnStartTime()
    {
        // Arrange
        var startTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var session = new Session { StartTime = startTime };

        // Act & Assert
        session.ScheduledStartTime.Should().Be(startTime);
    }

    [Fact]
    public void ScheduledStartTime_WhenSet_ShouldUpdateStartTime()
    {
        // Arrange
        var session = new Session();
        var newStartTime = new DateTime(2024, 1, 1, 14, 0, 0);

        // Act
        session.ScheduledStartTime = newStartTime;

        // Assert
        session.StartTime.Should().Be(newStartTime);
    }

    [Fact]
    public void ScheduledStartTime_WhenSetToNull_ShouldSetStartTimeToMinValue()
    {
        // Arrange
        var session = new Session();

        // Act
        session.ScheduledStartTime = null;

        // Assert
        session.StartTime.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void Room_ShouldGetAndSetLocation()
    {
        // Arrange
        var session = new Session();
        var roomName = "Conference Room A";

        // Act
        session.Room = roomName;

        // Assert
        session.Location.Should().Be(roomName);
        session.Room.Should().Be(roomName);
    }

    [Fact]
    public void Type_ShouldGetAndSetSessionType()
    {
        // Arrange
        var session = new Session();
        var sessionType = "Workshop";

        // Act
        session.Type = sessionType;

        // Assert
        session.SessionType.Should().Be(sessionType);
        session.Type.Should().Be(sessionType);
    }

    [Theory]
    [InlineData(SessionStatus.Proposed)]
    [InlineData(SessionStatus.UnderReview)]
    [InlineData(SessionStatus.Accepted)]
    [InlineData(SessionStatus.Rejected)]
    [InlineData(SessionStatus.Scheduled)]
    [InlineData(SessionStatus.Cancelled)]
    [InlineData(SessionStatus.Completed)]
    public void Status_ShouldSupportAllValidValues(SessionStatus status)
    {
        // Arrange
        var session = new Session();

        // Act
        session.Status = status;

        // Assert
        session.Status.Should().Be(status);
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var session = new Session();
        var conferenceId = "conf123";
        var callForPaperId = "cfp456";
        var title = "Advanced C# Techniques";
        var description = "Learn advanced C# programming techniques";
        var startTime = new DateTime(2024, 6, 15, 10, 0, 0);
        var endTime = new DateTime(2024, 6, 15, 11, 0, 0);
        var track = "Backend";
        var level = "Advanced";
        var sessionType = "Talk";
        var maxAttendees = 100;
        var reviewNotes = "Great session proposal";
        var location = "Room A";

        // Act
        session.ConferenceId = conferenceId;
        session.CallForPaperId = callForPaperId;
        session.Title = title;
        session.Description = description;
        session.StartTime = startTime;
        session.EndTime = endTime;
        session.Track = track;
        session.Level = level;
        session.SessionType = sessionType;
        session.MaxAttendees = maxAttendees;
        session.ReviewNotes = reviewNotes;
        session.Location = location;
        session.SpeakerIds.Add("speaker1");
        session.Tags.Add("csharp");
        session.Tags.Add("programming");

        // Assert
        session.ConferenceId.Should().Be(conferenceId);
        session.CallForPaperId.Should().Be(callForPaperId);
        session.Title.Should().Be(title);
        session.Description.Should().Be(description);
        session.StartTime.Should().Be(startTime);
        session.EndTime.Should().Be(endTime);
        session.Track.Should().Be(track);
        session.Level.Should().Be(level);
        session.SessionType.Should().Be(sessionType);
        session.MaxAttendees.Should().Be(maxAttendees);
        session.ReviewNotes.Should().Be(reviewNotes);
        session.Location.Should().Be(location);
        session.SpeakerIds.Should().Contain("speaker1");
        session.Tags.Should().Contain("csharp");
        session.Tags.Should().Contain("programming");
    }
}