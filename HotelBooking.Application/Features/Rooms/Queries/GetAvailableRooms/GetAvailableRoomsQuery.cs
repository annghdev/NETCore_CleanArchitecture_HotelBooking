using MediatR;

namespace HotelBooking.Application.Features.Rooms.Queries.GetAvailableRooms;

public record GetAvailableRoomsQuery(
    DateTimeOffset From,
    DateTimeOffset To) : IRequest<IEnumerable<RoomVM>>;
