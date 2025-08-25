using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class PaymentTransaction : EntityBase<Guid>, IAggregateRoot
{
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public double Amount { get; set; }
    public PaymentAction Action { get; set; }
    public PaymentGateway Gateway { get; set; }
    public PaymentProcessStatus ProcessStatus { get; set; }
    public string? TransactionNo { get; set; }
    public DateTimeOffset? OccuredDate { get; set; }
}
public enum PaymentGateway
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
public enum PaymentAction
{
    Prepay,
    CheckOut,
    Service,
    Other,
}
