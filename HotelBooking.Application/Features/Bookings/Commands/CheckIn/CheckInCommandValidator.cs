using FluentValidation;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckIn;

public class CheckInCommandValidator : AbstractValidator<CheckInCommand>
{
    public CheckInCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("ID booking không được để trống");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Tên khách hàng không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên khách hàng không được vượt quá 100 ký tự");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Số điện thoại không được để trống")
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .WithMessage("Số điện thoại không hợp lệ");

        RuleFor(x => x.ActualCheckInTime)
            .LessThanOrEqualTo(DateTimeOffset.Now.AddMinutes(30))
            .WithMessage("Thời gian check-in không thể ở tương lai quá 30 phút")
            .When(x => x.ActualCheckInTime.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
