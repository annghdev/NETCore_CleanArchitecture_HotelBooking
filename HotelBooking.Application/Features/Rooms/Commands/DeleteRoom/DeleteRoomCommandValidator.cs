using FluentValidation;

namespace HotelBooking.Application.Features.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandValidator : AbstractValidator<DeleteRoomCommand>
{
    public DeleteRoomCommandValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0");
    }
}
