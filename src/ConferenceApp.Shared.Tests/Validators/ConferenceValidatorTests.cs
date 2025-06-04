using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ConferenceApp.Shared.Tests.Validators;

public class ConferenceValidatorTests
{
    private readonly ConferenceValidator _validator;

    public ConferenceValidatorTests()
    {
        _validator = new ConferenceValidator();
    }

    [Fact]
    public void Validate_WithValidConference_ShouldPassValidation()
    {
        // Arrange
        var conference = new Conference
        {
            Name = "Tech Conference 2024",
            Description = "The biggest tech conference",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(32),
            Website = "https://techconf2024.com"
        };

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Name_WhenNullOrEmpty_ShouldHaveValidationError(string name)
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Name = name;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Conference name is required");
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Name = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Description_WhenNullOrEmpty_ShouldHaveValidationError(string description)
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Description = description;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Description = new string('a', 2001); // Exceeds 2000 character limit

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 2000 characters");
    }

    [Fact]
    public void StartDate_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.StartDate = default;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date is required");
    }

    [Fact]
    public void EndDate_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.EndDate = default;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("End date is required");
    }

    [Fact]
    public void EndDate_WhenBeforeStartDate_ShouldHaveValidationError()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.StartDate = new DateTime(2024, 6, 15);
        conference.EndDate = new DateTime(2024, 6, 14);

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("End date must be after or equal to start date");
    }

    [Fact]
    public void EndDate_WhenEqualToStartDate_ShouldPassValidation()
    {
        // Arrange
        var conference = CreateValidConference();
        var date = new DateTime(2024, 6, 15);
        conference.StartDate = date;
        conference.EndDate = date;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [Theory]
    [InlineData("https://techconf.com")]
    [InlineData("http://techconf.com")]
    [InlineData("https://www.techconf2024.org")]
    public void Website_WithValidUrls_ShouldPassValidation(string website)
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Website = website;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://invalid.com")]
    [InlineData("techconf.com")]
    [InlineData("www.techconf.com")]
    public void Website_WithInvalidUrls_ShouldHaveValidationError(string website)
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Website = website;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Website)
            .WithErrorMessage("Website must be a valid URL");
    }

    [Fact]
    public void Website_WhenNull_ShouldPassValidation()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Website = null;

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Fact]
    public void Website_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var conference = CreateValidConference();
        conference.Website = "";

        // Act
        var result = _validator.TestValidate(conference);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    private Conference CreateValidConference()
    {
        return new Conference
        {
            Name = "Tech Conference 2024",
            Description = "The biggest tech conference of the year",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(32),
            Website = "https://techconf2024.com"
        };
    }
}