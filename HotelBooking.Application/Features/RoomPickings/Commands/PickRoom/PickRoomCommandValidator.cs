using FluentValidation;

namespace HotelBooking.Application.Features.RoomPickings.Commands.PickRoom;

public class PickRoomCommandValidator : AbstractValidator<PickRoomCommand>
{
    public PickRoomCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("ID phòng phải là số dương");

        RuleFor(x => x.Origin)
            .IsInEnum()
            .WithMessage("Nguồn booking không hợp lệ");

        RuleFor(x => x.BookingType)
            .IsInEnum()
            .WithMessage("Loại booking không hợp lệ");

        RuleFor(x => x.CheckInDateTime)
            .NotEmpty()
            .WithMessage("Thời gian check-in không được để trống")
            .GreaterThan(DateTimeOffset.Now.AddMinutes(-30))
            .WithMessage("Thời gian check-in không được ở quá khứ quá 30 phút");

        RuleFor(x => x.CheckOutDateTime)
            .NotEmpty()
            .WithMessage("Thời gian check-out không được để trống")
            .GreaterThan(x => x.CheckInDateTime)
            .WithMessage("Thời gian check-out phải sau thời gian check-in");

        RuleFor(x => x.SessionId)
            .MaximumLength(100)
            .WithMessage("Session ID không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.SessionId));
    }
}
