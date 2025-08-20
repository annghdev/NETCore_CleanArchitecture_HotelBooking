using HotelBooking.Persistence.DbContexts;

namespace HotelBooking.API.Implements;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid? UserId => Guid.Parse(_contextAccessor.HttpContext?.User.FindFirst("UserId")?.Value);
}
