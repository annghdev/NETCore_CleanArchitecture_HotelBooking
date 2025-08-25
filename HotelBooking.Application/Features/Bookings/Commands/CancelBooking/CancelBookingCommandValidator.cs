using FluentValidation;

namespace HotelBooking.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("ID booking không được để trống");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Lý do hủy booking không được để trống")
            .MaximumLength(500)
            .WithMessage("Lý do hủy booking không được vượt quá 500 ký tự");
    }
}
