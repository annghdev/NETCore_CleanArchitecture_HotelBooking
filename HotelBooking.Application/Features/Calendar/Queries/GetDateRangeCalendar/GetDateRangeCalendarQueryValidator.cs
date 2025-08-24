using FluentValidation;

namespace HotelBooking.Application.Features.Calendar.Queries.GetDateRangeCalendar;

public class GetDateRangeCalendarQueryValidator : AbstractValidator<GetDateRangeCalendarQuery>
{
    public GetDateRangeCalendarQueryValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0");

        RuleFor(x => x.From)
            .NotEmpty().WithMessage("From date is required");

        RuleFor(x => x.To)
            .NotEmpty().WithMessage("To date is required")
            .GreaterThanOrEqualTo(x => x.From).WithMessage("To date must be greater than or equal to From date");

        RuleFor(x => x)
            .Must(x => (x.To.ToDateTime(TimeOnly.MinValue) - x.From.ToDateTime(TimeOnly.MinValue)).Days <= 31)
            .WithMessage("Date range cannot exceed 31 days");
    }
}
