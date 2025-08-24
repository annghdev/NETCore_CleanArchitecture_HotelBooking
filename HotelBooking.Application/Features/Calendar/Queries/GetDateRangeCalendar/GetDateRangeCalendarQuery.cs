using HotelBooking.Application.ReadModels;
using MediatR;

namespace HotelBooking.Application.Features.Calendar.Queries.GetDateRangeCalendar;

public record GetDateRangeCalendarQuery(
    int RoomId,
    DateOnly From,
    DateOnly To) : IRequest<DateRangeCalendarMatrix>;
