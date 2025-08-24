using FluentValidation;
using HotelBooking.Application.Common.Helpers;

namespace HotelBooking.Application.Features.Rooms.Commands.CheckAvailable;

public class CheckRoomAvailableCommandValidator : AbstractValidator<CheckRoomAvailableCommand>
{
    public CheckRoomAvailableCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0");

        RuleFor(x => x.From)
            .NotEmpty().WithMessage("From time is required")
            .MustBeVietnamTime()
            .MustBeFutureVietnamTime(-30); // Allow 30 minutes in the past

        RuleFor(x => x.To)
            .MustBeVietnamTime()
            .Must((command, to) => 
            {
                if (!to.HasValue) return true; // To is optional
                var normalizedFrom = TimeZoneHelper.EnsureVietnamTimeZone(command.From);
                var normalizedTo = TimeZoneHelper.EnsureVietnamTimeZone(to.Value);
                return normalizedTo > normalizedFrom;
            })
            .WithMessage("To time must be after From time")
            .When(x => x.To.HasValue);

        RuleFor(x => x)
            .Must(x => {
                if (!x.To.HasValue) return true;
                var normalizedFrom = TimeZoneHelper.EnsureVietnamTimeZone(x.From);
                var normalizedTo = TimeZoneHelper.EnsureVietnamTimeZone(x.To.Value);
                return (normalizedTo - normalizedFrom).TotalHours <= 24;
            })
            .WithMessage("Duration cannot exceed 24 hours")
            .When(x => x.To.HasValue);
    }
}
