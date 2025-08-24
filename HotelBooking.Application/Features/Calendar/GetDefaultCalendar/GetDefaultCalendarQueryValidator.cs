using FluentValidation;
using HotelBooking.Application.Common.Helpers;

namespace HotelBooking.Application.Features.Calendar.GetDefaultCalendar;

public class GetDefaultCalendarQueryValidator : AbstractValidator<GetDefaultCalendarQuery>
{
    public GetDefaultCalendarQueryValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .MustBeVietnamTime();
    }
}
