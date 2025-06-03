using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for CallForPaper entity
/// </summary>
public class CallForPaperValidator : AbstractValidator<CallForPaper>
{
    public CallForPaperValidator()
    {
        RuleFor(x => x.ConferenceId)
            .NotEmpty().WithMessage("Conference ID is required");
            
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
            
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThan(x => x.Deadline).WithMessage("Start date must be before the deadline");
            
        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Deadline is required");
            
        RuleFor(x => x.Topics)
            .NotNull().WithMessage("Topics collection cannot be null");
            
        RuleFor(x => x.SessionTypes)
            .NotNull().WithMessage("Session types collection cannot be null")
            .Must(st => st.Count > 0).WithMessage("At least one session type must be specified");
            
        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("A valid contact email is required")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}
