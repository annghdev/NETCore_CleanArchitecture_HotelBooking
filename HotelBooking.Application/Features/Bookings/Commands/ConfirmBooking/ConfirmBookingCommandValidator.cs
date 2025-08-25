using FluentValidation;

namespace HotelBooking.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("ID booking không được để trống");

        RuleFor(x => x.CustomerName)
            .MaximumLength(100)
            .WithMessage("Tên khách hàng không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.CustomerName));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .WithMessage("Số điện thoại không hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
