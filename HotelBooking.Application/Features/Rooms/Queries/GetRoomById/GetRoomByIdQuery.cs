using MediatR;

namespace HotelBooking.Application.Features.Rooms.Queries.GetRoomById;

public record GetRoomByIdQuery(int Id) : IRequest<RoomVM?>;

