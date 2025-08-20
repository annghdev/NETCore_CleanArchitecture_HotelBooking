using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Bookings;

public class BookingVM
{
    public Guid Id { get; set; }
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

    public virtual ICollection<PaymentTransactionVM>? Payments { get; set; }
}

public class PaymentTransactionVM
{
    public DateTimeOffset? OccuredDate { get; set; }
    public double Amount { get; set; }
    public PaymentType Type { get; set; }
    public PaymentOrigin Origin { get; set; }
    public string? TransactionNo { get; set; }
    public PaymentProcessStatus ProcessStatus { get; set; }
}
