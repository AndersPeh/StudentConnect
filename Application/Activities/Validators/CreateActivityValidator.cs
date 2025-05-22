using System;
using Application.Activities.Commands;
using FluentValidation;

namespace Application.Activities.Validators;

// validate the Command of CreateActivity which is public required CreateActivityDto ActivityDto { get; set; }.
// specify <CreateActivity.Command> for DI container to know it has to use this validator to validate Command in CreateActivity.cs.
public class CreateActivityValidator : AbstractValidator<CreateActivity.Command>
{
    // how this validator should validate.
    public CreateActivityValidator()
    {
        RuleFor(x => x.ActivityDto.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.ActivityDto.Description).NotEmpty().WithMessage("Description is required");

    }
}
