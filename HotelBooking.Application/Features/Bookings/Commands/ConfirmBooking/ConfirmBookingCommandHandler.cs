using AutoMapper;

namespace HotelBooking.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, BookingVM>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public ConfirmBookingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IBookingRepository bookingRepository, IRoomRepository roomRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }


    /// <summary>
    /// Xác nhận booking từ trạng thái Pending sang Confirmed
    /// Thường được gọi sau khi payment thành công hoặc admin xác nhận thủ công
    /// </summary>
    public async Task<BookingVM> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        // Lấy booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException("Booking", request.BookingId.ToString());

        // Kiểm tra trạng thái booking
        if (booking.Status != BookingStatus.Pending)
            throw new DomainException($"Chỉ có thể xác nhận booking ở trạng thái Pending. Trạng thái hiện tại: {booking.Status}");

        // Validate lại tính khả dụng của phòng
        await ValidateRoomsStillAvailable(booking);

        // Validate payment status nếu cần
        ValidatePaymentStatus(booking, request.SkipPaymentValidation);

        // Cập nhật thông tin booking
        booking.Status = BookingStatus.Confirmed;

        // Cập nhật thông tin customer nếu có
        if (!string.IsNullOrEmpty(request.CustomerName))
        {
            booking.CustomerName = request.CustomerName;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            booking.PhoneNumber = request.PhoneNumber;
        }

        // Ghi notes xác nhận
        if (!string.IsNullOrEmpty(request.Notes))
        {
            booking.Notes = string.IsNullOrEmpty(booking.Notes)
                ? request.Notes
                : $"{booking.Notes}\n[Xác nhận] {request.Notes}";
        }

        await _bookingRepository.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Gửi notification xác nhận booking
        // await _notificationService.SendBookingConfirmedNotification(booking);

        return _mapper.Map<BookingVM>(booking);
    }

    /// <summary>
    /// Kiểm tra các phòng vẫn còn khả dụng
    /// </summary>
    private async Task ValidateRoomsStillAvailable(Booking booking)
    {
        if (!booking.CheckInDateTime.HasValue || !booking.CheckOutDateTime.HasValue)
            throw new DomainException("Thời gian check-in và check-out không hợp lệ");

        foreach (var bookingRoom in booking.Rooms ?? [])
        {
            var room = await _roomRepository.GetByIdAsync(bookingRoom.RoomId);
            if (room == null || !room.IsActive)
                throw new DomainException($"Phòng {bookingRoom.RoomId} không khả dụng");

            // Kiểm tra conflict với các booking khác đã confirmed (loại trừ booking hiện tại)
            var conflictBookings = _bookingRepository.GetQueryable()
                .Where(b => b.Id != booking.Id &&
                           b.Rooms!.Any(br => br.RoomId == bookingRoom.RoomId) &&
                           b.Status == BookingStatus.Confirmed &&
                           b.CheckInDateTime < booking.CheckOutDateTime &&
                           b.CheckOutDateTime > booking.CheckInDateTime)
                .Any();

            if (conflictBookings)
                throw new DomainException($"Phòng {room.Name} đã được xác nhận cho booking khác trong cùng thời gian");
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán
    /// </summary>
    private void ValidatePaymentStatus(Booking booking, bool skipPaymentValidation)
    {
        if (skipPaymentValidation)
            return;

        // Kiểm tra nếu booking yêu cầu thanh toán trước
        if (booking.PrepayAmount > 0)
        {
            // Kiểm tra có transaction thành công không
            var hasSuccessfulPayment = _bookingRepository.GetQueryable()
                .Where(b => b.Id == booking.Id)
                .SelectMany(b => b.Transactions!)
                .Any(t => t.Action == PaymentAction.Prepay &&
                         t.ProcessStatus == PaymentProcessStatus.Success &&
                         t.Amount >= booking.PrepayAmount);

            if (!hasSuccessfulPayment)
            {
                // Cập nhật payment status
                booking.PaymentStatus = PaymentStatus.UnPaid;
                throw new DomainException("Booking chưa được thanh toán đủ tiền cọc");
            }
            else
            {
                // Cập nhật payment status
                booking.PaymentStatus = PaymentStatus.Deposited;
            }
        }
    }
}
