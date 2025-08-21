using MediatR;

namespace HotelBooking.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid Id) : IRequest<UserProfileVM>;