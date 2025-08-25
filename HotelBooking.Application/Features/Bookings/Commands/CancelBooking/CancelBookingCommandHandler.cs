using AutoMapper;

namespace HotelBooking.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, BookingVM>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public CancelBookingCommandHandler(IUnitOfWork unitOfWork, IBookingRepository bookingRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }


    /// <summary>
    /// Hủy booking và xử lý refund nếu cần
    /// </summary>
    public async Task<BookingVM> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // Lấy booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new NotFoundException("Booking", request.BookingId.ToString());

        // Kiểm tra trạng thái booking có thể hủy không
        ValidateBookingCanBeCancelled(booking);

        // Kiểm tra thời gian hủy
        ValidateCancellationTiming(booking, request.ForceCancel);


        // Cập nhật booking
        booking.Status = BookingStatus.Cancelled;

        // Ghi lý do hủy
        var cancellationNote = $"[Hủy booking] Lý do: {request.Reason}";


        booking.Notes = string.IsNullOrEmpty(booking.Notes) 
            ? cancellationNote 
            : $"{booking.Notes}\n{cancellationNote}";

        await _bookingRepository.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Gửi notification hủy booking
        // await _notificationService.SendBookingCancelledNotification(booking, refundAmount);

        return _mapper.Map<BookingVM>(booking);
    }

    /// <summary>
    /// Kiểm tra booking có thể hủy không
    /// </summary>
    private void ValidateBookingCanBeCancelled(Booking booking)
    {
        var cancellableStatuses = new[]
        {
            BookingStatus.Draft,
            BookingStatus.Pending,
            BookingStatus.Confirmed
        };

        if (!cancellableStatuses.Contains(booking.Status))
        {
            throw new DomainException($"Không thể hủy booking ở trạng thái {booking.Status}");
        }

        // Không thể hủy booking đã check-in
        if (booking.CheckedInAt.HasValue)
        {
            throw new DomainException("Không thể hủy booking đã check-in");
        }
    }

    /// <summary>
    /// Kiểm tra thời gian hủy booking
    /// </summary>
    private void ValidateCancellationTiming(Booking booking, bool forceCancel)
    {
        if (forceCancel)
            return; // Admin có thể force cancel

        if (!booking.CheckInDateTime.HasValue)
            return;

        // Không thể hủy booking trong vòng 2 giờ trước check-in
        var timeDifference = booking.CheckInDateTime.Value - DateTimeOffset.UtcNow;
        if (timeDifference.TotalHours < 2)
        {
            throw new DomainException("Không thể hủy booking trong vòng 2 giờ trước thời gian check-in");
        }
    }
}
