using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CalculateAmount;

public record CalculateAmountCommand(Guid Id) : IRequest<CalculatedAmountResult>;