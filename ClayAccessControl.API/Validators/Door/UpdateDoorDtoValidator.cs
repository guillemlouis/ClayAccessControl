using FluentValidation;
using ClayAccessControl.Core.DTOs;

namespace ClayAccessControl.API.Validators
{
    public class UpdateDoorDtoValidator : AbstractValidator<UpdateDoorDto>
    {
        public UpdateDoorDtoValidator()
        {
            RuleFor(x => x.DoorId)
                .GreaterThan(0).WithMessage("A valid door ID is required.");

            RuleFor(x => x.DoorName)
                .NotEmpty().WithMessage("Door name is required.")
                .MaximumLength(100).WithMessage("Door name must not exceed 100 characters.");

            RuleFor(x => x.RequiredAccessLevel)
                .InclusiveBetween(1, 5).WithMessage("Required access level must be between 1 and 5.");
        }
    }
}