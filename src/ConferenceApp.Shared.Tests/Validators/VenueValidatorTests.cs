using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ConferenceApp.Shared.Tests.Validators;

public class VenueValidatorTests
{
    private readonly VenueValidator _validator;

    public VenueValidatorTests()
    {
        _validator = new VenueValidator();
    }

    [Fact]
    public void Validate_WithValidVenue_ShouldPassValidation()
    {
        // Arrange
        var venue = new Venue
        {
            Name = "Tech Convention Center",
            Address = "123 Main Street",
            City = "San Francisco",
            State = "CA",
            ZipCode = "94102",
            Country = "USA",
            Capacity = 5000,
            Rooms = new List<Room>
            {
                new Room { Name = "Main Hall", Capacity = 1000 },
                new Room { Name = "Meeting Room A", Capacity = 50 }
            }
        };

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Name_WhenNullOrEmpty_ShouldHaveValidationError(string name)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Name = name;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Name = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Address_WhenNullOrEmpty_ShouldHaveValidationError(string address)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Address = address;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Address is required");
    }

    [Fact]
    public void Address_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Address = new string('a', 501); // Exceeds 500 character limit

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Address cannot exceed 500 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void City_WhenNullOrEmpty_ShouldHaveValidationError(string city)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.City = city;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City is required");
    }

    [Fact]
    public void City_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.City = new string('a', 101); // Exceeds 100 character limit

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void State_WhenNullOrEmpty_ShouldHaveValidationError(string state)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.State = state;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State)
            .WithErrorMessage("State is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ZipCode_WhenNullOrEmpty_ShouldHaveValidationError(string zipCode)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.ZipCode = zipCode;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ZipCode)
            .WithErrorMessage("Zip code is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Country_WhenNullOrEmpty_ShouldHaveValidationError(string country)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Country = country;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Capacity_WhenZeroOrNegative_ShouldHaveValidationError(int capacity)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Capacity = capacity;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Capacity)
            .WithErrorMessage("Capacity must be greater than 0");
    }

    [Fact]
    public void Rooms_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Rooms.Clear();

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rooms)
            .WithErrorMessage("At least one room must be defined");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void RoomName_WhenNullOrEmpty_ShouldHaveValidationError(string roomName)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Rooms[0].Name = roomName;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor("Rooms[0].Name")
            .WithErrorMessage("Room name is required");
    }

    [Fact]
    public void RoomName_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Rooms[0].Name = new string('a', 101); // Exceeds 100 character limit

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor("Rooms[0].Name")
            .WithErrorMessage("Room name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void RoomCapacity_WhenZeroOrNegative_ShouldHaveValidationError(int capacity)
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.Rooms[0].Capacity = capacity;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor("Rooms[0].Capacity")
            .WithErrorMessage("Room capacity must be greater than 0");
    }

    [Fact]
    public void ConferenceIds_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.ConferenceIds = null!;

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConferenceIds)
            .WithErrorMessage("ConferenceIds collection cannot be null");
    }

    [Fact]
    public void ConferenceIds_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var venue = CreateValidVenue();
        venue.ConferenceIds.Clear();

        // Act
        var result = _validator.TestValidate(venue);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ConferenceIds);
    }

    private Venue CreateValidVenue()
    {
        return new Venue
        {
            Name = "Tech Convention Center",
            Address = "123 Main Street",
            City = "San Francisco",
            State = "CA",
            ZipCode = "94102",
            Country = "USA",
            Capacity = 5000,
            Rooms = new List<Room>
            {
                new Room { Name = "Main Hall", Capacity = 1000 }
            }
        };
    }
}