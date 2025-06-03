using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for Attendee entity
/// </summary>
public class AttendeeValidator : AbstractValidator<Attendee>
{
    public AttendeeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email is required");
            
        RuleFor(x => x.Company)
            .MaximumLength(200).WithMessage("Company cannot exceed 200 characters");
            
        RuleFor(x => x.JobTitle)
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters");
            
        RuleFor(x => x.ConferenceRegistrations)
            .NotNull().WithMessage("ConferenceRegistrations dictionary cannot be null");
    }
}
