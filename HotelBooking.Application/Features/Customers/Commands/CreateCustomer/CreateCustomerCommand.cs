using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string FullName,
    string PhoneNumber,
    string? SessionId = null,
    string? IdentityNo = null) : IRequest<CustomerVM>;