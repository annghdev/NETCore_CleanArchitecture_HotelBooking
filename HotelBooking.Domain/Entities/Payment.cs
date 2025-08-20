using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class Payment : EntityBase<Guid>, IAggregateRoot
{
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public double Amount { get; set; }
    public PaymentType Type { get; set; }
    public PaymentOrigin Origin { get; set; }
    public PaymentProcessStatus ProcessStatus { get; set; }
    public string? TransactionNo { get; set; }
    public DateTimeOffset? OccuredDate { get; set; }
}
public enum PaymentOrigin
{
    Offline,
    BankTransfer,
    MoMo,
    VnPay
}
public enum PaymentProcessStatus
{
    Pending,
    Success,
    Failed,
    Refunded
}
public enum PaymentType
{
    Deposit,
    PartialPay,
    Checkout
}
