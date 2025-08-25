using AutoMapper;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, BookingVM>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CheckInCommandHandler(
        IUnitOfWork unitOfWork, 
        IBookingRepository bookingRepository, 
        ICustomerRepository customerRepository, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Xử lý check-in cho booking (lễ tân làm thủ tục nhận phòng)
    /// </summary>
    public async Task<BookingVM> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        // Lấy booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId) 
            ?? throw new NotFoundException("Booking", request.BookingId.ToString());

        // Kiểm tra trạng thái booking
        if (booking.Status != BookingStatus.Confirmed)
            throw new DomainException($"Chỉ có thể check-in cho booking đã được xác nhận. Trạng thái hiện tại: {booking.Status}");

        if (booking.CheckedInAt.HasValue)
            throw new DomainException("Booking này đã được check-in");

        // Kiểm tra thời gian check-in
        ValidateCheckInTime(booking, request.ActualCheckInTime);

        // Cập nhật thông tin customer nếu có
        await UpdateCustomerInfo(booking, request);

        // Cập nhật booking
        booking.Status = BookingStatus.CheckedIn;
        booking.CheckedInAt = request.ActualCheckInTime ?? DateTimeOffset.UtcNow;

        // Ghi notes check-in
        var checkInNote = $"[Check-in] Thời gian: {booking.CheckedInAt:dd/MM/yyyy HH:mm}";
        if (!string.IsNullOrEmpty(request.Notes))
        {
            checkInNote += $" | Ghi chú: {request.Notes}";
        }

        booking.Notes = string.IsNullOrEmpty(booking.Notes) 
            ? checkInNote 
            : $"{booking.Notes}\n{checkInNote}";

        await _bookingRepository.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Gửi notification check-in thành công
        // await _notificationService.SendCheckInNotification(booking);

        return _mapper.Map<BookingVM>(booking);
    }

    /// <summary>
    /// Kiểm tra thời gian check-in hợp lệ
    /// </summary>
    private void ValidateCheckInTime(Booking booking, DateTimeOffset? actualCheckInTime)
    {
        var checkInTime = actualCheckInTime ?? DateTimeOffset.UtcNow;

        // Không thể check-in quá sớm (trước 6 giờ so với thời gian dự kiến)
        if (booking.CheckInDateTime.HasValue)
        {
            var timeDifference = booking.CheckInDateTime.Value - checkInTime;
            if (timeDifference.TotalHours > 6)
            {
                throw new DomainException("Không thể check-in quá sớm so với thời gian dự kiến");
            }
        }

        // Không thể check-in sau thời gian check-out
        if (booking.CheckOutDateTime.HasValue && checkInTime >= booking.CheckOutDateTime.Value)
        {
            throw new DomainException("Thời gian check-in không thể sau thời gian check-out");
        }
    }

    /// <summary>
    /// Cập nhật thông tin customer
    /// </summary>
    private async Task UpdateCustomerInfo(Booking booking, CheckInCommand request)
    {
        // Cập nhật thông tin customer nếu có
        if (!string.IsNullOrEmpty(request.CustomerName))
        {
            booking.CustomerName = request.CustomerName;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            booking.PhoneNumber = request.PhoneNumber;
        }

        // Nếu có customer ID, cập nhật thông tin customer entity
        if (booking.CustomerId.HasValue && 
            (!string.IsNullOrEmpty(request.CustomerName) || !string.IsNullOrEmpty(request.PhoneNumber)))
        {
            var customer = await _customerRepository.GetByIdAsync(booking.CustomerId.Value);
            if (customer != null)
            {
                if (!string.IsNullOrEmpty(request.CustomerName))
                {
                    customer.FullName = request.CustomerName;
                }

                if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    customer.PhoneNumber = request.PhoneNumber;
                }

                await _customerRepository.UpdateAsync(customer);
            }
        }
    }
}
