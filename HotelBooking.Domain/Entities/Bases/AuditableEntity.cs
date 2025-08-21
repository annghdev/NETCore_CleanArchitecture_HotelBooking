namespace HotelBooking.Domain.Entities.Bases;

public abstract class AuditableEntity<TKey> : EntityBase<TKey>, IAuditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
}
