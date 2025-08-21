using MediatR;

namespace HotelBooking.Application.Features.PricingPolicies.Commands.UpdatePricingPolicy;

public record UpdatePricingPolicyCommand(
    int Id,
    string Name,
    double BasePrice, 
    double ExtraUnitPrice,
    int? MinDuration,
    int? MaxDuration,
    TimeOnly CheckInTimeDefault,
    TimeOnly CheckOutTimeDefault) : IRequest;
