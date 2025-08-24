using HotelBooking.Application.Features.Payments;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Bookings;

public class BookingVM
{
    public Guid Id { get; set; }
    public int RoomId { get; set; }
    public string? RoomName { get; set; }

    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }

    public double DepositAmount { get; set; }
    public double OriginalAmount { get; set; }
    public double DiscountAmount { get; set; }
    public double FinalAmount { get; set; }

    public BookingType Type { get; set; }
    public string? TypeName { get; set; }
    public BookingOrigin Origin { get; set; }
    public string? OriginName { get; set; }
    public DateTimeOffset? CheckInDateTime { get; set; }
    public DateTimeOffset? CheckOutDateTime { get; set; }

    public DateTimeOffset? CheckedInAt { get; set; }
    public DateTimeOffset? CheckedOutAt { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentStatusName { get; set; }
    public BookingStatus Status { get; set; }
    public string? StatusName { get; set; }
    public virtual IEnumerable<BookingRoomVM> Rooms { get; set; } = [];
    public virtual IEnumerable<PaymentTransactionVM> Transactions { get; set; } = [];
}

public class BookingRoomVM
{
    public Guid Id { get; set; }
    public int RoomId { get; set; }
    public string? RoomName { get; set; }
    public double? SubTotal { get; set; }
    public string? Notes { get; set; }
    public Guid? ChangedToRoomId { get; set; }
    public string? ChangedToRoom { get; set; }
    public DateTimeOffset? ChangedRoomDate { get; set; }
}