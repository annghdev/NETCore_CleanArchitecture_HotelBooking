using HotelBooking.Application;
using HotelBooking.Infrastructure;
using HotelBooking.Persistence;
using HotelBooking.Persistence.DbContexts;

namespace HotelBooking.API;

public static class ServicesRegistration
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddApplicationServices();
        services.AddPersistenceServics(configuration);
        services.AddInfrastructureServices(configuration);
    }
}
