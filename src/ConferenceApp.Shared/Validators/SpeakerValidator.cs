using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for Speaker entity
/// </summary>
public class SpeakerValidator : AbstractValidator<Speaker>
{
    public SpeakerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
            
        RuleFor(x => x.Bio)
            .NotEmpty().WithMessage("Bio is required")
            .MaximumLength(2000).WithMessage("Bio cannot exceed 2000 characters");
            
        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company is required")
            .MaximumLength(200).WithMessage("Company cannot exceed 200 characters");
            
        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("Job title is required")
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email is required");
            
        RuleFor(x => x.Website)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
            .WithMessage("Website must be a valid URL");
            
        RuleFor(x => x.ConferenceIds)
            .NotNull().WithMessage("ConferenceIds collection cannot be null");
    }
    
    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;
            
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && 
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
