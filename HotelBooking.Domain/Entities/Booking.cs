using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class Booking : AuditableEntity<Guid>, IAggregateRoot
{
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public Customer? Customer { get; set; }
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }

    public double PrepayAmount { get; set; }
    public double OriginalAmount { get; set; }
    public double DiscountAmount { get; set; }
    public double FinalAmount { get; set; }

    public BookingType Type { get; set; }
    public BookingOrigin Origin { get; set; }
    public DateTimeOffset? CheckInDateTime { get; set; }
    public DateTimeOffset? CheckOutDateTime { get; set; }

    public DateTimeOffset? CheckedInAt { get; set; }
    public DateTimeOffset? CheckedOutAt { get; set; }
    public DateTimeOffset? DraftedDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public virtual ICollection<BookingRoom>? Rooms { get; set; }
    public virtual ICollection<PaymentTransaction>? Transactions { get; set; }
}

public enum BookingOrigin
{
    Direct,
    Web,
    Mobile
}

public enum BookingStatus
{
    Draft,
    Pending,
    Confirmed,
    CheckedIn,
    CheckedOut,
    NoShow,
    Cancelled,
}

public enum BookingType
{
    Hourly,
    OverNight,
    Daily,
}

public enum PaymentStatus
{
    UnPaid,
    Deposited,
    Paid
}
