namespace HotelBooking.Application.Features.RoomPickings.Commands.ReleasePickedRoom;

public record ReleasePickedRoomCommand(Guid PickingId, int RoomId) : IRequest<bool>;
