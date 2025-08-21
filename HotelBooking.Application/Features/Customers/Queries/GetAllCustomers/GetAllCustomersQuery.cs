using MediatR;

namespace HotelBooking.Application.Features.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery : IRequest<IEnumerable<CustomerVM>>;
