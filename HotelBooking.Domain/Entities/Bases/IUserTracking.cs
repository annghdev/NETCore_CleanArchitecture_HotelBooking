namespace HotelBooking.Domain.Entities.Bases;

public interface IUserTracking
{
    Guid? CreatedBy { get; set; }
    Guid? UpdatedBy { get; set; }
}
