using ConferenceApp.Shared.Models;
using ConferenceApp.Shared.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ConferenceApp.Shared.Tests.Validators;

public class AttendeeValidatorTests
{
    private readonly AttendeeValidator _validator;

    public AttendeeValidatorTests()
    {
        _validator = new AttendeeValidator();
    }

    [Fact]
    public void Validate_WithValidAttendee_ShouldPassValidation()
    {
        // Arrange
        var attendee = new Attendee
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Company = "Tech Corp",
            JobTitle = "Software Engineer"
        };

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Name_WhenNullOrEmpty_ShouldHaveValidationError(string name)
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Name = name;

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Name = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Email_WhenNullOrEmpty_ShouldHaveValidationError(string email)
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Email = email;

        // Act
        var result = _validator.TestValidate(attendee);

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
        var attendee = CreateValidAttendee();
        attendee.Email = email;

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("A valid email is required");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("john.doe@company.org")]
    [InlineData("attendee@conference.com")]
    public void Email_WhenValid_ShouldPassValidation(string email)
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Email = email;

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Company_WhenNull_ShouldPassValidation()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Company = null;

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Fact]
    public void Company_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Company = "";

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Fact]
    public void Company_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.Company = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Company)
            .WithErrorMessage("Company cannot exceed 200 characters");
    }

    [Fact]
    public void JobTitle_WhenNull_ShouldPassValidation()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.JobTitle = null;

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.JobTitle);
    }

    [Fact]
    public void JobTitle_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.JobTitle = "";

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.JobTitle);
    }

    [Fact]
    public void JobTitle_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.JobTitle = new string('a', 201); // Exceeds 200 character limit

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobTitle)
            .WithErrorMessage("Job title cannot exceed 200 characters");
    }

    [Fact]
    public void ConferenceRegistrations_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.ConferenceRegistrations = null!;

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConferenceRegistrations)
            .WithErrorMessage("ConferenceRegistrations dictionary cannot be null");
    }

    [Fact]
    public void ConferenceRegistrations_WhenEmpty_ShouldPassValidation()
    {
        // Arrange
        var attendee = CreateValidAttendee();
        attendee.ConferenceRegistrations.Clear();

        // Act
        var result = _validator.TestValidate(attendee);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ConferenceRegistrations);
    }

    private Attendee CreateValidAttendee()
    {
        return new Attendee
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Company = "Tech Corp",
            JobTitle = "Software Engineer"
        };
    }
}