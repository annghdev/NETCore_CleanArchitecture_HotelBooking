namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID booking không được để trống");

        RuleFor(x => x.PrepayOrigin)
            .IsInEnum()
            .WithMessage("Phương thức thanh toán không hợp lệ");

        RuleFor(x => x.VoucherCode)
            .MaximumLength(50)
            .WithMessage("Mã voucher không được vượt quá 50 ký tự")
            .When(x => !string.IsNullOrEmpty(x.VoucherCode));
    }
}
