using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ConferenceApp.Shared.Tests.Validators;

public class SpeakerValidatorTests
{
    private readonly SpeakerValidator _validator;

    public SpeakerValidatorTests()
    {
        _validator = new SpeakerValidator();
    }

    [Fact]
    public void Validate_WithValidSpeaker_ShouldPassValidation()
    {
        // Arrange
        var speaker = new Speaker
        {
            Name = "John Doe",
            Bio = "Experienced software engineer with 10 years in the industry",
            Company = "Tech Corp",
            JobTitle = "Senior Software Engineer",
            Email = "john.doe@example.com",
            Website = "https://johndoe.dev"
        };

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Name_WhenNullOrEmpty_ShouldHaveValidationError(string name)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Name = name;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Name = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Bio_WhenNullOrEmpty_ShouldHaveValidationError(string bio)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Bio = bio;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio is required");
    }

    [Fact]
    public void Bio_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Bio = new string('a', 2001); // Exceeds 2000 character limit

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio cannot exceed 2000 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Company_WhenNullOrEmpty_ShouldHaveValidationError(string company)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Company = company;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Company)
            .WithErrorMessage("Company is required");
    }

    [Fact]
    public void Company_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Company = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Company)
            .WithErrorMessage("Company cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void JobTitle_WhenNullOrEmpty_ShouldHaveValidationError(string jobTitle)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.JobTitle = jobTitle;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobTitle)
            .WithErrorMessage("Job title is required");
    }

    [Fact]
    public void JobTitle_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.JobTitle = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobTitle)
            .WithErrorMessage("Job title cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Email_WhenNullOrEmpty_ShouldHaveValidationError(string email)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Email = email;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("not.an.email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Email_WhenInvalid_ShouldHaveValidationError(string email)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Email = email;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("A valid email is required");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("john.doe@company.org")]
    [InlineData("speaker@tech-conf.com")]
    public void Email_WhenValid_ShouldPassValidation(string email)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Email = email;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("https://johndoe.dev")]
    [InlineData("http://example.com")]
    [InlineData("https://www.speaker.org")]
    public void Website_WithValidUrls_ShouldPassValidation(string website)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Website = website;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://invalid.com")]
    [InlineData("johndoe.dev")]
    [InlineData("www.example.com")]
    public void Website_WithInvalidUrls_ShouldHaveValidationError(string website)
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Website = website;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Website)
            .WithErrorMessage("Website must be a valid URL");
    }

    [Fact]
    public void Website_WhenNull_ShouldPassValidation()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.Website = null;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Fact]
    public void ConferenceIds_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.ConferenceIds = null!;

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConferenceIds)
            .WithErrorMessage("ConferenceIds collection cannot be null");
    }

    [Fact]
    public void ConferenceIds_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var speaker = CreateValidSpeaker();
        speaker.ConferenceIds.Clear();

        // Act
        var result = _validator.TestValidate(speaker);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ConferenceIds);
    }

    private Speaker CreateValidSpeaker()
    {
        return new Speaker
        {
            Name = "John Doe",
            Bio = "Experienced software engineer",
            Company = "Tech Corp",
            JobTitle = "Senior Developer",
            Email = "john.doe@example.com",
            Website = "https://johndoe.dev"
        };
    }
}