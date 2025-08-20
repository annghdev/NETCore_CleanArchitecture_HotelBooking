using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Users;

public class UserProfileVM
{
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = default!;
    public string IdentityNo { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public AccountOrigin? AccountOrigin { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
}

public class CustomerProfileVM : UserProfileVM
{
    public Guid? CustomerId { get; set; }
}