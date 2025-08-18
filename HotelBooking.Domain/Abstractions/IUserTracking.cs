namespace HotelBooking.Domain.Abstractions;

public interface IUserTracking
{
    Guid? CreatedBy { get; set; }
    Guid? UpdatedBy { get; set; }
}
