using HotelBooking.Application.ReadModels;

namespace HotelBooking.Application.ReadServices;

public interface ICalendarService
{
    Task<CalendarMatrix> GetCalendarMatrixAsync(DateTime date);
}
