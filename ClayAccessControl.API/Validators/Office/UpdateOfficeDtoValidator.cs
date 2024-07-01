using FluentValidation;
using ClayAccessControl.Core.DTOs;

namespace ClayAccessControl.API.Validators
{
    public class UpdateOfficeDtoValidator : AbstractValidator<UpdateOfficeDto>
    {
        public UpdateOfficeDtoValidator()
        {
            RuleFor(x => x.OfficeName)
                .NotEmpty().WithMessage("Office name is required.")
                .MaximumLength(100).WithMessage("Office name must not exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");
        }
    }
}