using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for Conference entity
/// </summary>
public class ConferenceValidator : AbstractValidator<Conference>
{
    public ConferenceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Conference name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
            
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after or equal to start date");
            
        RuleFor(x => x.Website)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
            .WithMessage("Website must be a valid URL");
    }
    
    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;
            
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && 
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
