using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut.PreCheckOut;

public class PreCheckOutCommandHandler : IRequestHandler<PreCheckOutCommand, AmountResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;
    private readonly IAmountCalculator _amountCalculator;

    public PreCheckOutCommandHandler(IUnitOfWork unitOfWork, IBookingRepository bookingRepository, IAmountCalculator amountCalculator)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
        _amountCalculator = amountCalculator;
    }


    /// <summary>
    /// Tính toán số tiền cần thanh toán khi check-out
    /// </summary>
    public async Task<AmountResult> Handle(PreCheckOutCommand request, CancellationToken cancellationToken)
    {
        // Lấy booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new NotFoundException($"Booking",request.BookingId.ToString());

        // Kiểm tra trạng thái booking
        if (booking.Status != BookingStatus.CheckedIn)
            throw new DomainException($"Chỉ có thể check-out cho booking đã check-in. Trạng thái hiện tại: {booking.Status}");

        if (booking.CheckedOutAt.HasValue)
            throw new DomainException("Booking này đã được check-out");

        // Tính toán lại số tiền (có thể có thay đổi do early/late checkout)
        var amountResult = await _amountCalculator.CalculateAmount(booking);

        // Tính số tiền đã thanh toán
        var paidAmount = _bookingRepository.GetQueryable()
            .Where(b => b.Id == booking.Id)
            .SelectMany(b => b.Transactions!)
            .Where(t => t.ProcessStatus == PaymentProcessStatus.Success)
            .Sum(t => t.Amount);

        // Tính số tiền cần thanh toán thêm
        var remainingAmount = Math.Max(0, amountResult.TotalAmount - paidAmount);

        // Cập nhật lại amount trong result để hiển thị số tiền cần thanh toán
        if (remainingAmount != amountResult.TotalAmount)
        {
            // Tạo một room amount detail để hiển thị
            var paymentDetail = new RoomAmountDetail(
                DateTimeOffset.Now.ToString("dd/MM/yyyy"),
                "Check-out",
                "Thanh toán còn lại",
                null,
                null,
                remainingAmount,
                remainingAmount
            );

            var paymentRoom = new RoomAmount("Thanh toán", "Payment", [paymentDetail]);
            amountResult = new AmountResult([paymentRoom]);
        }

        return amountResult;
    }
}
