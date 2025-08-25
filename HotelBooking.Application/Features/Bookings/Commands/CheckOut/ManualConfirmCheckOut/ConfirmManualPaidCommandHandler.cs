namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut.ManualConfirmCheckOut;

public class ConfirmManualPaidCommandHandler : IRequestHandler<ConfirmManualPaidCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;

    /// <summary>
    /// Xác nhận thanh toán thủ công và hoàn tất check-out
    /// </summary>
    public async Task Handle(ConfirmManualPaidCommand request, CancellationToken cancellationToken)
    {
        // Lấy booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId) 
            ?? throw new NotFoundException($"Booking", request.BookingId.ToString());

        // Kiểm tra trạng thái booking
        if (booking.Status != BookingStatus.CheckedIn)
            throw new DomainException($"Chỉ có thể xác nhận thanh toán cho booking đã check-in. Trạng thái hiện tại: {booking.Status}");

        if (booking.CheckedOutAt.HasValue)
            throw new DomainException("Booking này đã được check-out");

        // Tạo transaction thanh toán thủ công
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Amount = request.Amount,
            Type = PaymentAction.CheckOut,
            Origin = request.Origin,
            ProcessStatus = PaymentProcessStatus.Success,
            TransactionNo = GenerateTransactionNo(),
            OccuredDate = DateTimeOffset.UtcNow
        };

        // Cập nhật booking
        booking.Status = BookingStatus.CheckedOut;
        booking.CheckedOutAt = DateTimeOffset.UtcNow;
        booking.PaymentStatus = PaymentStatus.Paid;

        // Ghi notes thanh toán và check-out
        var checkOutNote = $"[Check-out] Thời gian: {booking.CheckedOutAt:dd/MM/yyyy HH:mm} | Thanh toán thủ công: {request.Amount:N0} VND ({request.Origin})";
        booking.Notes = string.IsNullOrEmpty(booking.Notes) 
            ? checkOutNote 
            : $"{booking.Notes}\n{checkOutNote}";

        // Lưu thay đổi
        await _bookingRepository.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Gửi notification check-out thành công
        // await _notificationService.SendCheckOutNotification(booking);
    }

    /// <summary>
    /// Tạo transaction number
    /// </summary>
    private string GenerateTransactionNo()
    {
        return $"PAY{DateTimeOffset.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
