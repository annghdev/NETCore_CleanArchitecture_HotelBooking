using MediatR;

namespace HotelBooking.Application.Features.PricingPolicies.Queries.GetPricingPolicyById;

public record GetPricingPolicyByIdQuery(int Id) : IRequest<PricingPolicyVM>;
