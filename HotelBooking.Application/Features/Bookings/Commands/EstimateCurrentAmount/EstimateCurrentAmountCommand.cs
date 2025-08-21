using HotelBooking.Application.Features.Bookings.Commands.CalculateAmount;
using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.EstimateCurrentAmount;

public class EstimateCurrentAmountCommand(Guid Id) : IRequest<CalculatedAmountResult>; 