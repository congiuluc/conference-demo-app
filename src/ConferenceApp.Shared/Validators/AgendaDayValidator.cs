using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for AgendaDay entity
/// </summary>
public class AgendaDayValidator : AbstractValidator<AgendaDay>
{
    public AgendaDayValidator()
    {
        RuleFor(x => x.ConferenceId)
            .NotEmpty().WithMessage("Conference ID is required");
            
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");
            
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
            
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
            
        RuleFor(x => x.TimeSlotsByTrack)
            .NotNull().WithMessage("Time slots collection cannot be null");
            
        RuleForEach(x => x.TimeSlotsByTrack.Values).SetValidator(new TimeSlotListValidator());
    }
}

/// <summary>
/// Validator for a list of AgendaTimeSlot entities
/// </summary>
public class TimeSlotListValidator : AbstractValidator<List<AgendaTimeSlot>>
{
    public TimeSlotListValidator()
    {
        RuleForEach(x => x).SetValidator(new TimeSlotValidator());
    }
}

/// <summary>
/// Validator for AgendaTimeSlot entity
/// </summary>
public class TimeSlotValidator : AbstractValidator<AgendaTimeSlot>
{
    public TimeSlotValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required")
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
            
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required");
            
        RuleFor(x => x.SlotType)
            .NotEmpty().WithMessage("Slot type is required")
            .MaximumLength(50).WithMessage("Slot type cannot exceed 50 characters");
            
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required when no session is linked")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .When(x => string.IsNullOrEmpty(x.SessionId));
            
        // If this is a session time slot (not a break, etc.), require a venue and room
        When(x => x.SlotType == "Session" || x.SlotType == "Workshop" || x.SlotType == "Keynote", () => {
            RuleFor(x => x.VenueId)
                .NotEmpty().WithMessage("Venue ID is required for session time slots");
                
            RuleFor(x => x.Room)
                .NotEmpty().WithMessage("Room is required for session time slots");
        });
    }
}
