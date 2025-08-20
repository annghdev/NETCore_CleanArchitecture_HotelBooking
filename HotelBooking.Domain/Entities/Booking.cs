using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class Booking : AuditableEntity<Guid>, IAggregateRoot
{
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }

    public double DepositAmount { get; set; }
    public double OriginalAmount { get; set; }
    public double DiscountAmount { get; set; }
    public double FinalAmount { get; set; }

    public BookingType Type { get; set; }
    public BookingOrigin Origin { get; set; }

    public DateTime? CheckInDateTime { get; set; }
    public DateTime? CheckOutDateTime { get; set; }

    public DateTime? CheckedInAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public BookingStatus Status { get; set; }

    public virtual ICollection<Payment>? Payments { get; set; }
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
