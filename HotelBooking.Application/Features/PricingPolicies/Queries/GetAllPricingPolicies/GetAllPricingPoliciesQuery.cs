using MediatR;

namespace HotelBooking.Application.Features.PricingPolicies.Queries.GetAllPricingPolicies;

public record GetAllPricingPoliciesQuery : IRequest<IEnumerable<PricingPolicyVM>>;