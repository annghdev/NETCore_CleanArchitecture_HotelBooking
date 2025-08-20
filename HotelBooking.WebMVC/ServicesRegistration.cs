using HotelBooking.Application;
using HotelBooking.Infrastructure;
using HotelBooking.Persistence;

namespace HotelBooking.WebMVC;

public static class ServicesRegistration
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddPersistenceServics(configuration);
        services.AddInfrastructureServices(configuration);
    }
}
