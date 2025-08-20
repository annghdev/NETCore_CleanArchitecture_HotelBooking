using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using HotelBooking.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Persistence;

public static class PersistenceServiceRegistration
{
    public static void AddPersistenceServics(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });

        services.AddScoped<IUnitOfWork, BookingUnitOfWork>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPricingPolicyRepository, PricingPolicyRepository>();
        //services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
    }
}
