using FluentValidation;
using ClayAccessControl.Core.DTOs;

namespace ClayAccessControl.API.Validators
{
    public class GrantAccessDtoValidator : AbstractValidator<GrantAccessDto>
    {
        public GrantAccessDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("A valid user ID is required.");

            RuleFor(x => x.DoorId)
                .GreaterThan(0).WithMessage("A valid door ID is required.");
        }
    }
}