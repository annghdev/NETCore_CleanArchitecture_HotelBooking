using HotelBooking.Application.Common.Utils;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Queries.PaginCustomers;

public class PaginCustomersQuery : PaginRequest, IRequest<PagedResult<CustomerVM>>
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset? CreatedDateTo { get; set; }
    public SearchDateType SearchDateType { get; set; } = SearchDateType.After;
}
