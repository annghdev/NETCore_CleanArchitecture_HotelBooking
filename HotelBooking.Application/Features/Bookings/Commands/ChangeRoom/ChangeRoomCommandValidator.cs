using FluentValidation;

namespace HotelBooking.Application.Features.Bookings.Commands.ChangeRoom;

public class ChangeRoomCommandValidator : AbstractValidator<ChangeRoomCommand>
{
    public ChangeRoomCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("ID booking không được để trống");

        RuleFor(x => x.FromRoomId)
            .GreaterThan(0)
            .WithMessage("ID phòng cũ phải là số dương");

        RuleFor(x => x.ToRoomId)
            .GreaterThan(0)
            .WithMessage("ID phòng mới phải là số dương")
            .NotEqual(x => x.FromRoomId)
            .WithMessage("Phòng mới phải khác phòng cũ");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Lý do đổi phòng không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}
