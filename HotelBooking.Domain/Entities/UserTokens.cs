using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class UserTokens : EntityBase<Guid>
{
    public Guid UserId { get; set; } = default!;
    public virtual User? User { get; set; } = default!;
    public DateTimeOffset CreatedDate { get; set; } = default!;
    public string Value { get; set; } = default!;
    public TokenType Type { get; set; } = default!;
    public DateTimeOffset ExpiryDate { get; set; } = default!;
    public string? DevideId { get; set; }
    public string? IPAddress { get; set; }
}

public enum TokenType
{
    AccessToken,
    RefreshToken
}