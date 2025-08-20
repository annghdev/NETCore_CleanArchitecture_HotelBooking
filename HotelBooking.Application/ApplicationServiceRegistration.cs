using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HotelBooking.Application;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
