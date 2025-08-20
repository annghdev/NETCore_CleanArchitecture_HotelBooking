using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.AddAccountForExistedCustomer;

public record AddAccountForExistedCustomerCommand(Guid CustomerId, Guid UserId) : IRequest<bool>;
