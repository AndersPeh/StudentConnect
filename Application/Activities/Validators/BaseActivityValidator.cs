using System;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators;

// Because BaseActivityValidator will handle different kind of Commands from various activities like CreateActivity.Command,
// specify <T> for AbstractValidator to handle generic Commands, refers to the type of object the validator will validate. For example, CreateActivity.Command.
// TDto will be an instance of BaseActivityDto or a class derived from the Command (ActivityDto), a property of <T> for checking its properties like Title using RuleFor.
// BaseActivityValidator takes <T> and <TDto>, extracts properties of TDto from <T> to validate.

public class BaseActivityValidator<T, TDto> : AbstractValidator<T> where TDto : BaseActivityDto
{
    // In the primary constructor of BaseActivityValidator, there is a generic function named selector that takes generic type argument <T> 
    // like Command and returns TDto for extracting its property like Title.
    public BaseActivityValidator(Func<T, TDto> selector)
    {
        // x => selector(x).propertyName: x refers to Command (T) from Mediator, selector access the ActivityDto (TDto) in the Command.
        // selector(x) returns an instance of TDto then lambda extracts its properties from TDto for validation.
        // For example, RuleFor(CreateActivity.Command => CreateActivity.Command.ActivityDto.Title).
        RuleFor(x => selector(x).Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => selector(x).Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => selector(x).Date)
            .GreaterThan(DateTime.UtcNow).WithMessage("Date must be in the future");

        RuleFor(x => selector(x).Category)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => selector(x).City)
            .NotEmpty().WithMessage("City is required");

        RuleFor(x => selector(x).Venue)
            .NotEmpty().WithMessage("Venue is required");

        RuleFor(x => selector(x).Latitude)
            .NotEmpty().WithMessage("Latitude is required")
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => selector(x).Longitude)
            .NotEmpty().WithMessage("Longitude is required")
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
    }
}
