using MediatR;

namespace HotelBooking.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUserQuery : IRequest<IEnumerable<UserVM>>;