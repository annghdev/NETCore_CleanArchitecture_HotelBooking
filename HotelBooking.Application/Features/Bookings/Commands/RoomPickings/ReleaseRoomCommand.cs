namespace HotelBooking.Application.Features.Bookings.Commands.RoomPickings;

public record ReleaseRoomCommand(Guid PickingId, int RoomId) : IRequest<bool>;
