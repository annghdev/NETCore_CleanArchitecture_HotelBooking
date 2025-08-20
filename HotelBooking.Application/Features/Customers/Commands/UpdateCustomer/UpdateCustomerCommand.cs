using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid CustomerId,
    string Name,
    string PhoneNumber,
    string? IdentityNo = null,
    string? SessionId = null): IRequest;
