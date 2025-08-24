using MediatR;

namespace HotelBooking.Application.Features.Rooms.Queries.GetAllRooms;

public record GetAllRoomsQuery : IRequest<IEnumerable<RoomVM>>;