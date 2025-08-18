using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Repositories;

public interface IBookingRepository : IRepositoryBase<Booking, Guid>
{
}
