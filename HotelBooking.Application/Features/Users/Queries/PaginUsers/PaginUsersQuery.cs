using HotelBooking.Application.Common;
using HotelBooking.Domain.Common;
using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Users.Queries.PaginUsers;

public class PaginUsersQuery : PaginRequest, IRequest<PagedResult<UserVM>>
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public Gender? Gender { get; set; }
    public bool? IsConfirmed { get; set; }
    public bool? IsLocked { get; set; }
    public AccountOrigin? Origin { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset? CreatedDateTo { get; set; }
    public SearchDateType SearchDateType { get; set; } = SearchDateType.After;
}
