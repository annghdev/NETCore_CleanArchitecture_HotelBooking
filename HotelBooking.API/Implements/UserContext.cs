using HotelBooking.Persistence.DbContexts;

namespace HotelBooking.API.Implements;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userId = _contextAccessor?.HttpContext?.User?.FindFirst("UserId")?.Value;
            return Guid.TryParse(userId, out var guid) ? guid : (Guid?)null;
        }
    }
}
