using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for Session entity
/// </summary>
public class SessionValidator : AbstractValidator<Session>
{
    public SessionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
            
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required")
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
            
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");
            
        RuleFor(x => x.Track)
            .NotEmpty().WithMessage("Track is required")
            .MaximumLength(100).WithMessage("Track cannot exceed 100 characters");
            
        RuleFor(x => x.Level)
            .NotEmpty().WithMessage("Level is required")
            .Must(BeValidLevel).WithMessage("Level must be Beginner, Intermediate, or Advanced");
            
        RuleFor(x => x.SpeakerIds)
            .NotEmpty().WithMessage("At least one speaker must be assigned");
            
        RuleFor(x => x.ConferenceId)
            .NotEmpty().WithMessage("Conference ID is required");
            
        RuleFor(x => x.SessionType)
            .NotEmpty().WithMessage("Session type is required")
            .MaximumLength(50).WithMessage("Session type cannot exceed 50 characters");
    }
    
    private bool BeValidLevel(string level)
    {
        var validLevels = new[] { "Beginner", "Intermediate", "Advanced" };
        return validLevels.Contains(level, StringComparer.OrdinalIgnoreCase);
    }
}
