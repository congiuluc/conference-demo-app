using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class AgendaDayTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var agendaDay = new AgendaDay();

        // Assert
        agendaDay.PartitionKey.Should().Be("AgendaDay");
        agendaDay.TimeSlotsByTrack.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var agendaDay = new AgendaDay();
        var conferenceId = "conf123";
        var date = new DateTime(2024, 6, 15);
        var title = "Opening Day";
        var description = "The first day of the conference";

        // Act
        agendaDay.ConferenceId = conferenceId;
        agendaDay.Date = date;
        agendaDay.Title = title;
        agendaDay.Description = description;
        
        // Add time slots by track
        var timeSlot1 = new AgendaTimeSlot
        {
            SessionId = "session1",
            StartTime = date.AddHours(9),
            EndTime = date.AddHours(10),
            SlotType = "Session"
        };
        
        var timeSlot2 = new AgendaTimeSlot
        {
            Title = "Coffee Break",
            StartTime = date.AddHours(10),
            EndTime = date.AddHours(10.5),
            SlotType = "Break"
        };
        
        agendaDay.TimeSlotsByTrack["Backend"] = new List<AgendaTimeSlot> { timeSlot1 };
        agendaDay.TimeSlotsByTrack["General"] = new List<AgendaTimeSlot> { timeSlot2 };

        // Assert
        agendaDay.ConferenceId.Should().Be(conferenceId);
        agendaDay.Date.Should().Be(date);
        agendaDay.Title.Should().Be(title);
        agendaDay.Description.Should().Be(description);
        agendaDay.TimeSlotsByTrack.Should().HaveCount(2);
        agendaDay.TimeSlotsByTrack.Should().ContainKey("Backend");
        agendaDay.TimeSlotsByTrack.Should().ContainKey("General");
        agendaDay.TimeSlotsByTrack["Backend"].Should().Contain(timeSlot1);
        agendaDay.TimeSlotsByTrack["General"].Should().Contain(timeSlot2);
    }

    [Fact]
    public void TimeSlotsByTrack_ShouldSupportMultipleTracks()
    {
        // Arrange
        var agendaDay = new AgendaDay();
        var date = DateTime.Now;

        // Act
        agendaDay.TimeSlotsByTrack["Frontend"] = new List<AgendaTimeSlot>
        {
            new AgendaTimeSlot { SessionId = "session1", StartTime = date.AddHours(9), EndTime = date.AddHours(10), SlotType = "Session" },
            new AgendaTimeSlot { SessionId = "session2", StartTime = date.AddHours(11), EndTime = date.AddHours(12), SlotType = "Session" }
        };
        
        agendaDay.TimeSlotsByTrack["Backend"] = new List<AgendaTimeSlot>
        {
            new AgendaTimeSlot { SessionId = "session3", StartTime = date.AddHours(9), EndTime = date.AddHours(10), SlotType = "Session" }
        };

        // Assert
        agendaDay.TimeSlotsByTrack["Frontend"].Should().HaveCount(2);
        agendaDay.TimeSlotsByTrack["Backend"].Should().HaveCount(1);
    }
}

public class AgendaTimeSlotTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var timeSlot = new AgendaTimeSlot();

        // Assert
        timeSlot.SessionId.Should().BeNull();
        timeSlot.Title.Should().BeNull();
        timeSlot.VenueId.Should().BeNull();
        timeSlot.Room.Should().BeNull();
        timeSlot.Notes.Should().BeNull();
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var timeSlot = new AgendaTimeSlot();
        var sessionId = "session123";
        var title = "Advanced C# Techniques";
        var startTime = new DateTime(2024, 6, 15, 9, 0, 0);
        var endTime = new DateTime(2024, 6, 15, 10, 0, 0);
        var slotType = "Session";
        var venueId = "venue1";
        var room = "Conference Room A";
        var notes = "This session requires laptops";

        // Act
        timeSlot.SessionId = sessionId;
        timeSlot.Title = title;
        timeSlot.StartTime = startTime;
        timeSlot.EndTime = endTime;
        timeSlot.SlotType = slotType;
        timeSlot.VenueId = venueId;
        timeSlot.Room = room;
        timeSlot.Notes = notes;

        // Assert
        timeSlot.SessionId.Should().Be(sessionId);
        timeSlot.Title.Should().Be(title);
        timeSlot.StartTime.Should().Be(startTime);
        timeSlot.EndTime.Should().Be(endTime);
        timeSlot.SlotType.Should().Be(slotType);
        timeSlot.VenueId.Should().Be(venueId);
        timeSlot.Room.Should().Be(room);
        timeSlot.Notes.Should().Be(notes);
    }

    [Fact]
    public void TimeSlot_ForBreak_ShouldNotRequireSessionId()
    {
        // Arrange & Act
        var timeSlot = new AgendaTimeSlot
        {
            Title = "Coffee Break",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddMinutes(15),
            SlotType = "Break"
        };

        // Assert
        timeSlot.SessionId.Should().BeNull();
        timeSlot.Title.Should().Be("Coffee Break");
        timeSlot.SlotType.Should().Be("Break");
    }

    [Fact]
    public void TimeSlot_ForSession_ShouldHaveSessionId()
    {
        // Arrange & Act
        var timeSlot = new AgendaTimeSlot
        {
            SessionId = "session456",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1),
            SlotType = "Session"
        };

        // Assert
        timeSlot.SessionId.Should().Be("session456");
        timeSlot.SlotType.Should().Be("Session");
    }
}