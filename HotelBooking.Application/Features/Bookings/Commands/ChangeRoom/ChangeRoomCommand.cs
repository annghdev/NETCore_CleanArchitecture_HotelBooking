using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.ChangeRoom;

public record ChangeRoomCommand(Guid BookingId, Guid OldRoom, Guid NewRoom) : IRequest<ChangeRoomResult>;
