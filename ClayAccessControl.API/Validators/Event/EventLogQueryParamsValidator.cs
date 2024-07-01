using FluentValidation;
using ClayAccessControl.Core.DTOs;

namespace ClayAccessControl.API.Validators
{
    public class EventLogQueryParamsValidator : AbstractValidator<EventLogQueryParams>
    {
        public EventLogQueryParamsValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate).When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Start date must be before or equal to end date.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate).When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("End date must be after or equal to start date.");

            RuleFor(x => x.EventType)
                .MaximumLength(50).WithMessage("Event type must not exceed 50 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).When(x => x.UserId.HasValue)
                .WithMessage("User ID must be a positive integer.");

            RuleFor(x => x.DoorId)
                .GreaterThan(0).When(x => x.DoorId.HasValue)
                .WithMessage("Door ID must be a positive integer.");

            RuleFor(x => x.OfficeId)
                .GreaterThan(0).When(x => x.OfficeId.HasValue)
                .WithMessage("Office ID must be a positive integer.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).When(x => x.PageNumber.HasValue)
                .WithMessage("Page number must be a positive integer.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).When(x => x.PageSize.HasValue)
                .WithMessage("Page size must be a positive integer.");
        }
    }
}