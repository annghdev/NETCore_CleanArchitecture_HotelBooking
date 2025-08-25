using AutoMapper;
using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IBusinessSettingRepository _businessSettingRepository;
    private readonly IMapper _mapper;
    private readonly IAmountCalculator _amountCalculator;

    public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IBusinessSettingRepository businessSettingRepository,
        IMapper mapper,
        IAmountCalculator amountCalculator)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _amountCalculator = amountCalculator;
        _businessSettingRepository = businessSettingRepository;
    }


    /// <summary>
    /// Tạo booking chính thức từ draft booking và xử lý payment
    /// </summary>
    public async Task<CreateBookingResult> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy draft booking
            var draftBooking = await _bookingRepository.GetByIdAsync(request.Id);
            if (draftBooking == null)
                return CreateBookingResult.Failure("Không tìm thấy booking draft");

            if (draftBooking.Status != BookingStatus.Draft)
                return CreateBookingResult.Failure("Booking không ở trạng thái draft");

            // Kiểm tra draft booking còn hiệu lực không
            await ValidateDraftBookingExpiry(draftBooking);

            // Validate lại tính khả dụng của phòng
            await ValidateRoomsStillAvailable(draftBooking);

            // Lấy business setting
            var businessSetting = await _businessSettingRepository.GetSetting()
                ?? throw new DomainException("Không tìm thấy cài đặt hệ thống");

            // Tính toán lại số tiền (có thể có thay đổi pricing)
            var amountResult = await _amountCalculator.CalculateAmount(draftBooking);

            // Áp dụng voucher nếu có
            var discountAmount = await ApplyVoucherDiscount(request.VoucherCode, amountResult.TotalAmount);

            var finalAmount = amountResult.TotalAmount - discountAmount;
            var depositAmount = CalculateDepositAmount(finalAmount, businessSetting.PrepayPercent);

            // Cập nhật booking
            draftBooking.Status = BookingStatus.Pending;
            draftBooking.OriginalAmount = amountResult.TotalAmount;
            draftBooking.DiscountAmount = discountAmount;
            draftBooking.FinalAmount = finalAmount;
            draftBooking.PrepayAmount = depositAmount;

            // Tạo payment requirement
            RequirePaymentResult? paymentResult = null;
            if (depositAmount > 0)
            {
                paymentResult = await CreatePaymentRequirement(
                    draftBooking,
                    depositAmount,
                    request.PrepayOrigin);
            }

            await _bookingRepository.UpdateAsync(draftBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var bookingVm = _mapper.Map<BookingVM>(draftBooking);

            return CreateBookingResult.Success(bookingVm, amountResult, paymentResult!);
        }
        catch (Exception ex) when (ex is not DomainException and not NotFoundException)
        {
            return CreateBookingResult.Failure($"Lỗi khi tạo booking: {ex.Message}");
        }
    }

    /// <summary>
    /// Kiểm tra draft booking còn hiệu lực không
    /// </summary>
    private async Task ValidateDraftBookingExpiry(Booking draftBooking)
    {
        var businessSetting = await _businessSettingRepository.GetSetting()
            ?? throw new DomainException("Không tìm thấy cài đặt hệ thống");

        if (draftBooking.DraftedDate.HasValue)
        {
            var expiryTime = draftBooking.DraftedDate.Value.AddMinutes(businessSetting.DraftExpiryMinutes);
            if (DateTimeOffset.UtcNow > expiryTime)
            {
                throw new DomainException("Booking draft đã hết hạn, vui lòng tạo lại");
            }
        }
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

            // Kiểm tra conflict (loại trừ booking hiện tại)
            var conflictBookings = _bookingRepository.GetQueryable()
                .Where(b => b.Id != booking.Id &&
                           b.Rooms!.Any(br => br.RoomId == bookingRoom.RoomId) &&
                           b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Draft &&
                           b.CheckInDateTime < booking.CheckOutDateTime &&
                           b.CheckOutDateTime > booking.CheckInDateTime)
                .Any();

            if (conflictBookings)
                throw new DomainException($"Phòng {room.Name} đã được đặt bởi booking khác");
        }
    }

    /// <summary>
    /// Áp dụng voucher giảm giá
    /// </summary>
    private async Task<double> ApplyVoucherDiscount(string? voucherCode, double originalAmount)
    {
        if (string.IsNullOrEmpty(voucherCode))
            return 0;

        // TODO: Implement voucher logic
        // Tạm thời return 0, sẽ implement sau khi có voucher module
        return await Task.FromResult(0);
    }

    /// <summary>
    /// Tính tiền cọc cần thanh toán
    /// </summary>
    private double CalculateDepositAmount(double finalAmount, int prepayPercent)
    {
        return Math.Round(finalAmount * prepayPercent / 100.0, 0);
    }

    /// <summary>
    /// Tạo yêu cầu thanh toán
    /// </summary>
    private async Task<RequirePaymentResult> CreatePaymentRequirement(
        Booking booking,
        double amount,
        PaymentGateway paymentGateway)
    {
        // Tạo payment transaction
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Amount = amount,
            Action = PaymentAction.Prepay,
            Gateway = paymentGateway,
            ProcessStatus = PaymentProcessStatus.Pending,
            TransactionNo = string.Empty,
            OccuredDate = DateTimeOffset.UtcNow
        };

        // TODO: Tích hợp payment gateway để tạo payment link
        string? paymentLink = null;
        string? gateway = null;
        bool isRedirect = false;
        int timeout = 15;

        switch (paymentGateway)
        {
            case PaymentGateway.MoMo:
                // TODO: Implement MoMo integration
                gateway = "MoMo";
                isRedirect = true;
                break;
            case PaymentGateway.VnPay:
                // TODO: Implement VnPay integration
                gateway = "VnPay";
                isRedirect = true;
                break;
            case PaymentGateway.BankTransfer:
                gateway = "Bank Transfer";
                isRedirect = false;
                break;
            case PaymentGateway.Offline:
                gateway = "Offline";
                isRedirect = false;
                break;
        }

        return new RequirePaymentResult(amount, paymentLink, gateway, isRedirect, timeout);
    }
}
