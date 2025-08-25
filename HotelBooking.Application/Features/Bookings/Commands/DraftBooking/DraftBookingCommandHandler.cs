using AutoMapper;
using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.DraftBooking;

public class DraftBookingCommandHandler : IRequestHandler<DraftBookingCommand, DraftBookingResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly IAmountCalculator _amountCalculator;
    private readonly IBusinessSettingRepository _businessSettingRepository;

    public DraftBookingCommandHandler(
        IUnitOfWork unitOfWork, 
        IRoomRepository roomRepository, 
        IBookingRepository bookingRepository, 
        IMapper mapper, 
        IAmountCalculator amountCalculator, 
        IBusinessSettingRepository businessSettingRepository)
    {
        _unitOfWork = unitOfWork;
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _amountCalculator = amountCalculator;
        _businessSettingRepository = businessSettingRepository;
    }


    /// <summary>
    /// Tạo booking draft để người dùng xem trước thông tin và số tiền trước khi xác nhận
    /// </summary>
    public async Task<DraftBookingResult> Handle(DraftBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate phòng tồn tại và khả dụng
            await ValidateRoomsAvailability(request.RoomIds, request.CheckInDateTime, request.CheckOutDateTime);

            // Lấy thông tin Business Setting để tính expiry time
            var businessSetting = await _businessSettingRepository.GetSetting() 
                ?? throw new DomainException("Không tìm thấy cài đặt hệ thống");

            // Chuyển đổi timezone từ VN sang UTC để lưu database
            var checkInUtc = TimeZoneHelper.ToUtc(request.CheckInDateTime);

            var checkOutUtc = TimeZoneHelper.ToUtc(request.CheckOutDateTime);

            // Tạo booking entity với status Draft
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                Type = request.Type,
                Origin = request.Origin,
                CheckInDateTime = checkInUtc,
                CheckOutDateTime = checkOutUtc,
                Status = BookingStatus.Draft,
                PaymentStatus = PaymentStatus.UnPaid,
                DraftedDate = DateTimeOffset.UtcNow,
                CreatedDate = DateTimeOffset.UtcNow,
            };

            // Thêm các phòng vào booking
            var bookingRooms = new List<BookingRoom>();
            foreach (var roomId in request.RoomIds)
            {
                var room = await _roomRepository.GetByIdAsync(roomId);
                if (room == null)
                    throw new NotFoundException("Room", roomId.ToString());

                bookingRooms.Add(new BookingRoom
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    RoomId = roomId,
                    Room = room,
                    CreatedDate = DateTimeOffset.UtcNow,
                });
            }

            booking.Rooms = bookingRooms;

            // Tính toán số tiền chi tiết
            var amountResult = await _amountCalculator.CalculateAmount(booking);
            
            booking.OriginalAmount = amountResult.TotalAmount;
            booking.FinalAmount = amountResult.TotalAmount; // Chưa có discount
            booking.DiscountAmount = 0;

            // Lưu booking draft vào database
            await _bookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Tính expiry time cho draft
            var expiryTime = DateTimeOffset.UtcNow.AddMinutes(businessSetting.DraftExpiryMinutes);
            
            // Convert về timezone VN cho response
            var bookingVm = _mapper.Map<BookingVM>(booking);

            return DraftBookingResult.Success(amountResult, bookingVm, expiryTime);
        }
        catch (Exception ex) when (ex is not DomainException and not NotFoundException)
        {
            return DraftBookingResult.Failure($"Lỗi khi tạo booking draft: {ex.Message}");
        }
    }

    /// <summary>
    /// Kiểm tra tính khả dụng của các phòng trong khoảng thời gian đặt
    /// </summary>
    private async Task ValidateRoomsAvailability(IEnumerable<int> roomIds, DateTimeOffset checkIn, DateTimeOffset checkOut)
    {
        if (checkIn >= checkOut)
            throw new DomainException("Thời gian check-in phải nhỏ hơn check-out");

        // Chuyển về UTC để query database
        var checkInUtc = TimeZoneHelper.ToUtc(checkIn);
        var checkOutUtc = TimeZoneHelper.ToUtc(checkOut);

        foreach (var roomId in roomIds)
        {
            // Kiểm tra phòng có tồn tại không
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new NotFoundException($"Room", roomId.ToString());

            if (!room.IsActive)
                throw new DomainException($"Phòng {room.Name} hiện không khả dụng");

            // Kiểm tra phòng có bị trùng lịch không
            var conflictBookings = _bookingRepository.GetQueryable()
                .Where(b => b.Rooms!.Any(br => br.RoomId == roomId) &&
                           b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Draft &&
                           b.CheckInDateTime < checkOutUtc &&
                           b.CheckOutDateTime > checkInUtc)
                .Any();

            if (conflictBookings)
                throw new DomainException($"Phòng {room.Name} đã được đặt trong khoảng thời gian này");
        }
    }
}
