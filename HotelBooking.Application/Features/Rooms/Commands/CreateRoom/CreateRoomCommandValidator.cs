using FluentValidation;

namespace HotelBooking.Application.Features.Rooms.Commands.CreateRoom;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Required")
            .MaximumLength(5).WithMessage("Max length is 5");
    }
}
