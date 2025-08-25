namespace HotelBooking.Application.Features.Bookings.Commands.DraftBooking;

public class DraftBookingCommandValidator : AbstractValidator<DraftBookingCommand>
{
    public DraftBookingCommandValidator()
    {
        RuleFor(x => x.RoomIds)
            .NotEmpty()
            .WithMessage("Phải chọn ít nhất một phòng")
            .Must(roomIds => roomIds.All(id => id > 0))
            .WithMessage("ID phòng phải là số dương");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Loại booking không hợp lệ");

        RuleFor(x => x.Origin)
            .IsInEnum()
            .WithMessage("Nguồn booking không hợp lệ");

        RuleFor(x => x.CheckInDateTime)
            .NotNull()
            .WithMessage("Thời gian check-in không được để trống")
            .WithMessage("Thời gian check-in phải ở múi giờ Việt Nam (+07:00)")
            .GreaterThan(DateTime.Now.AddMinutes(-30))
            .WithMessage("Thời gian check-in không được ở quá khứ quá 30 phút");

        RuleFor(x => x.CheckOutDateTime)
            .NotNull()
            .WithMessage("Thời gian check-out không được để trống")
            .WithMessage("Thời gian check-out phải ở múi giờ Việt Nam (+07:00)");

        RuleFor(x => x)
            .Must(x => x.CheckOutDateTime > x.CheckInDateTime)
            .WithMessage("Thời gian check-out phải sau thời gian check-in");

        // Validate theo loại booking
        RuleFor(x => x)
            .Must(ValidateBookingDuration)
            .WithMessage("Thời gian booking không phù hợp với loại booking được chọn");

        RuleFor(x => x.SessionId)
            .MaximumLength(100)
            .WithMessage("Session ID không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.SessionId));
    }

    /// <summary>
    /// Validate thời gian booking theo loại booking
    /// </summary>
    private bool ValidateBookingDuration(DraftBookingCommand command)
    {
        var duration = command.CheckOutDateTime - command.CheckInDateTime;

        return command.Type switch
        {
            BookingType.Hourly => duration.TotalHours >= 1 && duration.TotalHours <= 12,
            BookingType.OverNight => duration.TotalHours >= 6 && duration.TotalHours <= 18,
            BookingType.Daily => duration.TotalDays >= 1 && duration.TotalDays <= 30,
            _ => false
        };
    }
}
