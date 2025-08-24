using FluentValidation;
using HotelBooking.Application.Common;

namespace HotelBooking.Application.Features.Calendar.Queries.GetBookingCalendar;

public class GetBookingCalendarQueryValidator : AbstractValidator<GetBookingCalendarQuery>
{
    public GetBookingCalendarQueryValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .MustBeVietnamTime();
    }
}
