using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid CustomerId) : IRequest;
