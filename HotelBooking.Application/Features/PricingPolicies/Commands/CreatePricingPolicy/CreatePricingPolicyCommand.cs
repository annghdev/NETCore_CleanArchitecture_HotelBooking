using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.PricingPolicies.Commands.CreatePricingPolicy;

public record CreatePricingPolicyCommand(
    string Name,
    RoomType RoomType,
    BookingType BookingType,
    double BasePrice,
    double ExtraUnitPrice,
    int? MinDuration,
    int? MaxDuration,
    string? DaysOfWeek,
    TimeOnly CheckInTimeDefault,
    TimeOnly CheckOutTimeDefault) : IRequest<PricingPolicyVM>;
