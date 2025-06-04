using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class ConferenceTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var conference = new Conference();

        // Assert
        conference.PartitionKey.Should().Be("Conference");
        conference.IsActive.Should().BeTrue();
        conference.Categories.Should().NotBeNull().And.BeEmpty();
        conference.VenueIds.Should().NotBeNull().And.BeEmpty();
        conference.MaxAttendees.Should().Be(0);
        conference.CurrentAttendees.Should().Be(0);
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var conference = new Conference();
        var name = "Tech Conference 2024";
        var description = "The biggest tech conference of the year";
        var startDate = new DateTime(2024, 6, 15);
        var endDate = new DateTime(2024, 6, 17);
        var website = "https://techconf2024.com";
        var logoUrl = "https://example.com/logo.png";
        var location = "San Francisco, CA";
        var maxAttendees = 1000;
        var currentAttendees = 500;
        var organizer = "Tech Events Inc.";

        // Act
        conference.Name = name;
        conference.Description = description;
        conference.StartDate = startDate;
        conference.EndDate = endDate;
        conference.Website = website;
        conference.LogoUrl = logoUrl;
        conference.IsActive = false;
        conference.Location = location;
        conference.MaxAttendees = maxAttendees;
        conference.CurrentAttendees = currentAttendees;
        conference.Organizer = organizer;
        conference.Categories.Add("Technology");
        conference.Categories.Add("Software");
        conference.VenueIds.Add("venue1");
        conference.VenueIds.Add("venue2");

        // Assert
        conference.Name.Should().Be(name);
        conference.Description.Should().Be(description);
        conference.StartDate.Should().Be(startDate);
        conference.EndDate.Should().Be(endDate);
        conference.Website.Should().Be(website);
        conference.LogoUrl.Should().Be(logoUrl);
        conference.IsActive.Should().BeFalse();
        conference.Location.Should().Be(location);
        conference.MaxAttendees.Should().Be(maxAttendees);
        conference.CurrentAttendees.Should().Be(currentAttendees);
        conference.Organizer.Should().Be(organizer);
        conference.Categories.Should().Contain("Technology");
        conference.Categories.Should().Contain("Software");
        conference.VenueIds.Should().Contain("venue1");
        conference.VenueIds.Should().Contain("venue2");
    }

    [Fact]
    public void Categories_ShouldSupportMultipleValues()
    {
        // Arrange
        var conference = new Conference();

        // Act
        conference.Categories.Add("AI");
        conference.Categories.Add("Machine Learning");
        conference.Categories.Add("Cloud Computing");

        // Assert
        conference.Categories.Should().HaveCount(3);
        conference.Categories.Should().Contain(new[] { "AI", "Machine Learning", "Cloud Computing" });
    }

    [Fact]
    public void VenueIds_ShouldSupportMultipleValues()
    {
        // Arrange
        var conference = new Conference();

        // Act
        conference.VenueIds.Add("venue-001");
        conference.VenueIds.Add("venue-002");

        // Assert
        conference.VenueIds.Should().HaveCount(2);
        conference.VenueIds.Should().Contain(new[] { "venue-001", "venue-002" });
    }
}