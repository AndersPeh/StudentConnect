using System;
using Application.Activities.Commands;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators;

// The validator will process an instance of CreateActivity.Command provided via the Mediator pipeline.
// CreateActivityValidator inherits from BaseActivityValidator, specifying CreateActivity.Command as <T>, CreateActivityDto as <TDto>.
// By specifying BaseActivityValidator with CreateActivity.Command, CreateActivityValidator implements IValidator<CreateActivity.Command>.
// DI container knows it has to use CreateActivityValidator when ValidationBehavior requests a validator for CreateActivity.Command.
public class CreateActivityValidator : BaseActivityValidator<CreateActivity.Command, CreateActivityDto>
{
    // Because CreateActivity.Command has a property: public required CreateActivityDto ActivityDto { get; set; },
    // C# compiler knows for x.ActivityDto, the x must be CreateActivity.Command, so no need to specify x is CreateActivity.Command.
    // ActivityDto needs to be specified to be validated by base class rules.
    public CreateActivityValidator() : base(x => x.ActivityDto)
    {

    }
}
