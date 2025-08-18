using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.ReadModels;

public class CalendarCell
{
    public int TimeFrameId { get; set; }
    public Guid? BookingId { get; set; }
    public int ColSpan { get; set; } = 1;
    public BookingStatus BookingStatus { get; set; }
    public string? BookingStatusName { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;
    public string RoomStatusName => RoomStatus.ToDisplay();
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
}

public enum RoomStatus
{
    Unavailable,
    Available,
    Booked,
    Locked,
    Using,
    Maintenancing
}

public static class RoomStatusDisplay
{
    public static string ToDisplay(this RoomStatus roomStatus)
    {
        return roomStatus switch
        {
            RoomStatus.Unavailable => "-",
            RoomStatus.Available => "Trống",
            RoomStatus.Booked => "Đã đặt",
            RoomStatus.Using => "Đang sử dụng",
            RoomStatus.Locked => "Đang khóa",
            RoomStatus.Maintenancing => "Bảo trì",
            _ => "-"
        };
    }
}
