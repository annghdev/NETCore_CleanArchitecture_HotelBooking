using MediatR;

namespace HotelBooking.Application.Features.Calendar.Commands.CheckAvailable;

public record CheckRoomAvailableCommand(int RoomId, DateTimeOffset From, DateTimeOffset? To): IRequest<RoomAvailableResult>;
