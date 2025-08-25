using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Application.Features.RoomPickings.Repository;
using HotelBooking.Infrastructure.Auth;
using HotelBooking.Infrastructure.RoomPickings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenValidator, TokenValidator>();
        services.AddScoped<IRoomPickingRepository, RoomPickingRepository>();
    }
}
