using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class AttendeeTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var attendee = new Attendee();

        // Assert
        attendee.PartitionKey.Should().Be("Attendee");
        attendee.ConferenceRegistrations.Should().NotBeNull().And.BeEmpty();
        attendee.RegistrationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        attendee.IsConfirmed.Should().BeFalse();
    }

    [Fact]
    public void FirstName_WhenNameHasMultipleParts_ShouldReturnFirstPart()
    {
        // Arrange
        var attendee = new Attendee { Name = "John Doe Smith" };

        // Act & Assert
        attendee.FirstName.Should().Be("John");
    }

    [Fact]
    public void FirstName_WhenNameHasOnePart_ShouldReturnEntireName()
    {
        // Arrange
        var attendee = new Attendee { Name = "John" };

        // Act & Assert
        attendee.FirstName.Should().Be("John");
    }

    [Fact]
    public void FirstName_WhenNameIsNull_ShouldReturnEmpty()
    {
        // Arrange
        var attendee = new Attendee { Name = null! };

        // Act & Assert
        attendee.FirstName.Should().BeEmpty();
    }

    [Fact]
    public void FirstName_WhenSet_ShouldUpdateFirstPartOfName()
    {
        // Arrange
        var attendee = new Attendee { Name = "John Doe Smith" };

        // Act
        attendee.FirstName = "Jane";

        // Assert
        attendee.Name.Should().Be("Jane Doe Smith");
    }

    [Fact]
    public void FirstName_WhenSetWithSingleName_ShouldReplaceEntireName()
    {
        // Arrange
        var attendee = new Attendee { Name = "John" };

        // Act
        attendee.FirstName = "Jane";

        // Assert
        attendee.Name.Should().Be("Jane");
    }

    [Fact]
    public void LastName_WhenNameHasMultipleParts_ShouldReturnAllButFirst()
    {
        // Arrange
        var attendee = new Attendee { Name = "John Doe Smith" };

        // Act & Assert
        attendee.LastName.Should().Be("Doe Smith");
    }

    [Fact]
    public void LastName_WhenNameHasOnePart_ShouldReturnEmpty()
    {
        // Arrange
        var attendee = new Attendee { Name = "John" };

        // Act & Assert
        attendee.LastName.Should().BeEmpty();
    }

    [Fact]
    public void LastName_WhenNameIsNull_ShouldReturnEmpty()
    {
        // Arrange
        var attendee = new Attendee { Name = null! };

        // Act & Assert
        attendee.LastName.Should().BeEmpty();
    }

    [Fact]
    public void LastName_WhenSet_ShouldUpdateLastPartOfName()
    {
        // Arrange
        var attendee = new Attendee { Name = "John Doe" };

        // Act
        attendee.LastName = "Smith";

        // Assert
        attendee.Name.Should().Be("John Smith");
    }

    [Fact]
    public void LastName_WhenSetWithEmptyName_ShouldCreateNameWithFirstPartEmpty()
    {
        // Arrange
        var attendee = new Attendee { Name = "" };

        // Act
        attendee.LastName = "Smith";

        // Assert
        attendee.Name.Should().Be(" Smith");
    }

    [Fact]
    public void LastName_WhenSetWithNullName_ShouldCreateUnknownName()
    {
        // Arrange
        var attendee = new Attendee { Name = null! };

        // Act
        attendee.LastName = "Smith";

        // Assert
        attendee.Name.Should().Be("Unknown Smith");
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var attendee = new Attendee();
        var name = "Jane Doe";
        var email = "jane.doe@example.com";
        var company = "Tech Corp";
        var jobTitle = "Software Engineer";
        var phone = "+1-555-0123";
        var bio = "Experienced developer";
        var conferenceId = "conf123";
        var registrationDate = DateTime.UtcNow.AddDays(-1);

        // Act
        attendee.Name = name;
        attendee.Email = email;
        attendee.Company = company;
        attendee.JobTitle = jobTitle;
        attendee.Phone = phone;
        attendee.Bio = bio;
        attendee.ConferenceId = conferenceId;
        attendee.RegistrationDate = registrationDate;
        attendee.IsConfirmed = true;
        
        // Add conference registrations
        attendee.ConferenceRegistrations["conf1"] = new List<string> { "session1", "session2" };
        attendee.ConferenceRegistrations["conf2"] = new List<string> { "session3" };

        // Assert
        attendee.Name.Should().Be(name);
        attendee.Email.Should().Be(email);
        attendee.Company.Should().Be(company);
        attendee.JobTitle.Should().Be(jobTitle);
        attendee.Phone.Should().Be(phone);
        attendee.Bio.Should().Be(bio);
        attendee.ConferenceId.Should().Be(conferenceId);
        attendee.RegistrationDate.Should().Be(registrationDate);
        attendee.IsConfirmed.Should().BeTrue();
        attendee.ConferenceRegistrations.Should().ContainKey("conf1");
        attendee.ConferenceRegistrations["conf1"].Should().Contain("session1");
        attendee.ConferenceRegistrations["conf1"].Should().Contain("session2");
        attendee.ConferenceRegistrations["conf2"].Should().Contain("session3");
    }

    [Fact]
    public void ConferenceRegistrations_ShouldSupportMultipleConferences()
    {
        // Arrange
        var attendee = new Attendee();

        // Act
        attendee.ConferenceRegistrations["conf1"] = new List<string> { "session1", "session2", "session3" };
        attendee.ConferenceRegistrations["conf2"] = new List<string> { "session4", "session5" };

        // Assert
        attendee.ConferenceRegistrations.Should().HaveCount(2);
        attendee.ConferenceRegistrations["conf1"].Should().HaveCount(3);
        attendee.ConferenceRegistrations["conf2"].Should().HaveCount(2);
    }
}