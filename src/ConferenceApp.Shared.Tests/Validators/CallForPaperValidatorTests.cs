using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ConferenceApp.Shared.Tests.Validators;

public class CallForPaperValidatorTests
{
    private readonly CallForPaperValidator _validator;

    public CallForPaperValidatorTests()
    {
        _validator = new CallForPaperValidator();
    }

    [Fact]
    public void Validate_WithValidCallForPaper_ShouldPassValidation()
    {
        // Arrange
        var callForPaper = new CallForPaper
        {
            ConferenceId = "conf123",
            Title = "Call for Papers - Tech Conference 2024",
            Description = "We are looking for innovative speakers",
            StartDate = DateTime.UtcNow.AddDays(-30),
            Deadline = DateTime.UtcNow.AddDays(30),
            ContactEmail = "cfp@conference.com",
            SessionTypes = { "Talk", "Workshop" }
        };

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ConferenceId_WhenNullOrEmpty_ShouldHaveValidationError(string conferenceId)
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.ConferenceId = conferenceId;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConferenceId)
            .WithErrorMessage("Conference ID is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Title_WhenNullOrEmpty_ShouldHaveValidationError(string title)
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Title = title;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Title_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Title = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(callForPaper);

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
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Description = description;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Description = new string('a', 2001); // Exceeds 2000 character limit

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 2000 characters");
    }

    [Fact]
    public void StartDate_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.StartDate = default;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date is required");
    }

    [Fact]
    public void StartDate_WhenAfterDeadline_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.StartDate = DateTime.UtcNow.AddDays(10);
        callForPaper.Deadline = DateTime.UtcNow.AddDays(5);

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date must be before the deadline");
    }

    [Fact]
    public void Deadline_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Deadline = default;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Deadline)
            .WithErrorMessage("Deadline is required");
    }

    [Fact]
    public void Topics_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Topics = null!;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Topics)
            .WithErrorMessage("Topics collection cannot be null");
    }

    [Fact]
    public void Topics_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.Topics.Clear();

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Topics);
    }

    [Fact]
    public void SessionTypes_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.SessionTypes = null!;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SessionTypes)
            .WithErrorMessage("Session types collection cannot be null");
    }

    [Fact]
    public void SessionTypes_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.SessionTypes.Clear();

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SessionTypes)
            .WithErrorMessage("At least one session type must be specified");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("not.an.email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void ContactEmail_WhenInvalid_ShouldHaveValidationError(string email)
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.ContactEmail = email;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail)
            .WithErrorMessage("A valid contact email is required");
    }

    [Theory]
    [InlineData("cfp@conference.com")]
    [InlineData("contact@example.org")]
    [InlineData("info@techconf.dev")]
    public void ContactEmail_WhenValid_ShouldPassValidation(string email)
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.ContactEmail = email;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void ContactEmail_WhenNull_ShouldPassValidation()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.ContactEmail = null;

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void ContactEmail_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var callForPaper = CreateValidCallForPaper();
        callForPaper.ContactEmail = "";

        // Act
        var result = _validator.TestValidate(callForPaper);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContactEmail);
    }

    private CallForPaper CreateValidCallForPaper()
    {
        return new CallForPaper
        {
            ConferenceId = "conf123",
            Title = "Call for Papers - Tech Conference 2024",
            Description = "We are looking for innovative speakers and interesting topics",
            StartDate = DateTime.UtcNow.AddDays(-30),
            Deadline = DateTime.UtcNow.AddDays(30),
            ContactEmail = "cfp@conference.com",
            SessionTypes = { "Talk", "Workshop" }
        };
    }
}