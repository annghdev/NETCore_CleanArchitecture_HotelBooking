using HotelBooking.Application.Common.Utils;
using MediatR;

namespace HotelBooking.Application.Features.Bookings.Queries.PaginBookings;

public class PaginBookingsCommand : PaginRequest, IRequest<PagedResult<BookingVM>>
{
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset? CreatedDateTo { get; set; }
    public SearchDateType SearchDateType { get; set; } = SearchDateType.After;
}
