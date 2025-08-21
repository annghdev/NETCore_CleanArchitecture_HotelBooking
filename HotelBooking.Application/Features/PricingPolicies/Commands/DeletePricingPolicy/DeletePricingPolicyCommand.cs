using MediatR;

namespace HotelBooking.Application.Features.PricingPolicies.Commands.DeletePricingPolicy;

public record DeletePricingPolicyCommand(int Id) : IRequest;
