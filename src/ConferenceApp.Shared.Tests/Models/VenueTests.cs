using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class VenueTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.PartitionKey.Should().Be("Venue");
        venue.ConferenceIds.Should().NotBeNull().And.BeEmpty();
        venue.Rooms.Should().NotBeNull().And.BeEmpty();
        venue.Capacity.Should().Be(0);
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var venue = new Venue();
        var name = "Tech Convention Center";
        var address = "123 Main Street";
        var city = "San Francisco";
        var state = "CA";
        var zipCode = "94102";
        var country = "USA";
        var capacity = 5000;

        // Act
        venue.Name = name;
        venue.Address = address;
        venue.City = city;
        venue.State = state;
        venue.ZipCode = zipCode;
        venue.Country = country;
        venue.Capacity = capacity;
        venue.ConferenceIds.Add("conf1");
        venue.ConferenceIds.Add("conf2");

        var room1 = new Room { Name = "Main Hall", Capacity = 1000 };
        var room2 = new Room { Name = "Meeting Room A", Capacity = 50 };
        venue.Rooms.Add(room1);
        venue.Rooms.Add(room2);

        // Assert
        venue.Name.Should().Be(name);
        venue.Address.Should().Be(address);
        venue.City.Should().Be(city);
        venue.State.Should().Be(state);
        venue.ZipCode.Should().Be(zipCode);
        venue.Country.Should().Be(country);
        venue.Capacity.Should().Be(capacity);
        venue.ConferenceIds.Should().Contain("conf1");
        venue.ConferenceIds.Should().Contain("conf2");
        venue.Rooms.Should().HaveCount(2);
        venue.Rooms.Should().Contain(room1);
        venue.Rooms.Should().Contain(room2);
    }
}

public class RoomTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var room = new Room();

        // Assert
        room.Id.Should().NotBeNullOrEmpty();
        Guid.TryParse(room.Id, out _).Should().BeTrue();
        room.Equipment.Should().NotBeNull().And.BeEmpty();
        room.Capacity.Should().Be(0);
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var room = new Room();
        var name = "Conference Room A";
        var capacity = 150;

        // Act
        room.Name = name;
        room.Capacity = capacity;
        room.Equipment.Add("Projector");
        room.Equipment.Add("Microphone");
        room.Equipment.Add("Whiteboard");

        // Assert
        room.Name.Should().Be(name);
        room.Capacity.Should().Be(capacity);
        room.Equipment.Should().HaveCount(3);
        room.Equipment.Should().Contain(new[] { "Projector", "Microphone", "Whiteboard" });
    }

    [Fact]
    public void Id_ShouldBeUniqueForEachInstance()
    {
        // Arrange & Act
        var room1 = new Room();
        var room2 = new Room();

        // Assert
        room1.Id.Should().NotBe(room2.Id);
    }
}