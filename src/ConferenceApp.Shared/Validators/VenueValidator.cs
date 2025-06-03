using ConferenceApp.Shared.Models;
using FluentValidation;

namespace ConferenceApp.Shared.Validators;

/// <summary>
/// Validator for Venue entity
/// </summary>
public class VenueValidator : AbstractValidator<Venue>
{
    public VenueValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
            
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
            
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");
            
        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters");
            
        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .MaximumLength(20).WithMessage("Zip code cannot exceed 20 characters");
            
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");
            
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");
            
        RuleFor(x => x.Rooms)
            .NotEmpty().WithMessage("At least one room must be defined");
            
        RuleForEach(x => x.Rooms).ChildRules(room => {
            room.RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Room name is required")
                .MaximumLength(100).WithMessage("Room name cannot exceed 100 characters");
                
            room.RuleFor(r => r.Capacity)
                .GreaterThan(0).WithMessage("Room capacity must be greater than 0");
        });
        
        RuleFor(x => x.ConferenceIds)
            .NotNull().WithMessage("ConferenceIds collection cannot be null");
    }
}
