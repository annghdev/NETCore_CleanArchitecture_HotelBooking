namespace HotelBooking.Application.Features.RoomPickings.Commands.PickRoom;

public record PickRoomCommand(
    int RoomId,
    Guid? CustomerId,
    string? SessionId, 
    BookingOrigin Origin, 
    BookingType BookingType,
    DateTimeOffset CheckInDateTime,
    DateTimeOffset? CheckOutDateTime,
    Guid? PickingId = null) : IRequest<bool>;
