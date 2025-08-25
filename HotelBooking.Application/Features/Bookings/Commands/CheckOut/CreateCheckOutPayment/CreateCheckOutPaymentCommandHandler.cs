namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut.CreateCheckOutPayment;

public class CreateCheckOutPaymentCommandHandler : IRequestHandler<CreateCheckOutPaymentCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;

    public CreateCheckOutPaymentCommandHandler(IUnitOfWork unitOfWork, IBookingRepository bookingRepository)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
    }


    /// <summary>
    /// Tạo payment link cho checkout payment
    /// </summary>
    public async Task<string> Handle(CreateCheckOutPaymentCommand request, CancellationToken cancellationToken)
    {
        // Lấy booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId) 
            ?? throw new NotFoundException("Booking", request.BookingId.ToString());

        // Kiểm tra trạng thái booking
        if (booking.Status != BookingStatus.CheckedIn)
            throw new DomainException($"Chỉ có thể tạo payment cho booking đã check-in. Trạng thái hiện tại: {booking.Status}");

        if (booking.CheckedOutAt.HasValue)
            throw new DomainException("Booking này đã được check-out");

        // Tính số tiền cần thanh toán
        var paidAmount = _bookingRepository.GetQueryable()
            .Where(b => b.Id == booking.Id)
            .SelectMany(b => b.Transactions!)
            .Where(t => t.ProcessStatus == PaymentProcessStatus.Success)
            .Sum(t => t.Amount);

        var remainingAmount = Math.Max(0, booking.FinalAmount - paidAmount);

        if (remainingAmount <= 0)
            throw new DomainException("Booking đã được thanh toán đầy đủ");

        // Tạo pending transaction
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Amount = remainingAmount,
            Type = PaymentAction.CheckOut,
            Origin = request.PaymentOrigin,
            ProcessStatus = PaymentProcessStatus.Pending,
            TransactionNo = GenerateTransactionNo(),
            OccuredDate = DateTimeOffset.UtcNow
        };

        // Tạo payment link dựa vào payment origin
        string paymentLink = await CreatePaymentLink(transaction, request.PaymentOrigin);

        // Lưu transaction
        // Note: Thường transaction sẽ được lưu trong payment gateway callback
        // Ở đây tạm thời comment out
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return paymentLink;
    }

    /// <summary>
    /// Tạo payment link với payment gateway
    /// </summary>
    private async Task<string> CreatePaymentLink(PaymentTransaction transaction, PaymentGateway paymentOrigin)
    {
        // TODO: Tích hợp với payment gateway
        switch (paymentOrigin)
        {
            case PaymentGateway.MoMo:
                // TODO: Implement MoMo payment link creation
                return $"https://momo.vn/payment?txn={transaction.TransactionNo}&amount={transaction.Amount}";

            case PaymentGateway.VnPay:
                // TODO: Implement VnPay payment link creation
                return $"https://vnpay.vn/payment?txn={transaction.TransactionNo}&amount={transaction.Amount}";

            case PaymentGateway.BankTransfer:
                // TODO: Generate QR code or bank transfer info
                return $"bank-transfer://{transaction.TransactionNo}";

            default:
                throw new DomainException($"Payment origin {paymentOrigin} không được hỗ trợ cho payment link");
        }
    }

    /// <summary>
    /// Tạo transaction number
    /// </summary>
    private string GenerateTransactionNo()
    {
        return $"CHK{DateTimeOffset.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
