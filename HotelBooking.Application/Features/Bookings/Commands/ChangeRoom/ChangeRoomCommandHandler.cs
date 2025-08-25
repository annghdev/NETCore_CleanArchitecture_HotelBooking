using AutoMapper;
using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.ChangeRoom;

public class ChangeRoomCommandHandler : IRequestHandler<ChangeRoomCommand, ChangeRoomResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly IAmountCalculator _amountCalculator;

    public ChangeRoomCommandHandler(
        IUnitOfWork unitOfWork, 
        IBookingRepository bookingRepository, 
        IRoomRepository roomRepository, 
        IMapper mapper, 
        IAmountCalculator amountCalculator)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _amountCalculator = amountCalculator;
    }

    /// <summary>
    /// Đổi phòng cho booking đã confirmed
    /// </summary>
    public async Task<ChangeRoomResult> Handle(ChangeRoomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy booking
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            if (booking == null)
                return ChangeRoomResult.Failure("Không tìm thấy booking");

            // Kiểm tra trạng thái booking có thể đổi phòng không
            if (booking.Status != BookingStatus.Confirmed)
                return ChangeRoomResult.Failure("Chỉ có thể đổi phòng cho booking đã xác nhận");

            if (booking.CheckedInAt.HasValue)
                return ChangeRoomResult.Failure("Không thể đổi phòng sau khi đã check-in");

            // Lấy booking room cần đổi
            var bookingRoom = booking.Rooms?.FirstOrDefault(br => br.RoomId == request.FromRoomId);
            if (bookingRoom == null)
                return ChangeRoomResult.Failure("Không tìm thấy phòng trong booking");

            // Kiểm tra phòng đích
            var newRoom = await _roomRepository.GetByIdAsync(request.ToRoomId);
            if (newRoom == null)
                return ChangeRoomResult.Failure("Không tìm thấy phòng đích");

            if (!newRoom.IsActive)
                return ChangeRoomResult.Failure("Phòng đích không khả dụng");

            // Kiểm tra phòng đích có trống không
            await ValidateNewRoomAvailability(booking, request.ToRoomId);

            // Tính toán chênh lệch giá (nếu có)
            var priceDifference = await CalculatePriceDifference(booking, request.FromRoomId, request.ToRoomId);

            // Tạo booking room mới
            var newBookingRoom = new BookingRoom
            {
                Id = Guid.CreateVersion7(),
                RoomId = request.ToRoomId,
            };

            booking.Rooms?.Add(newBookingRoom);

            // Cập nhật booking room
            bookingRoom.ChangedToRoomId = newBookingRoom.Id;
            bookingRoom.ChangedRoomDate = DateTimeOffset.UtcNow;

            // Ghi notes
            var changeNote = $"[Đổi phòng] Từ phòng {request.FromRoomId} sang phòng {request.ToRoomId}";
            if (!string.IsNullOrEmpty(request.Reason))
            {
                changeNote += $" | Lý do: {request.Reason}";
            }

            if (priceDifference != 0)
            {
                changeNote += $" | Chênh lệch giá: {priceDifference:N0} VND";
                booking.FinalAmount += priceDifference;
            }

            booking.Notes = string.IsNullOrEmpty(booking.Notes) 
                ? changeNote 
                : $"{booking.Notes}\n{changeNote}";

            booking.UpdatedDate = DateTimeOffset.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var bookingVm = _mapper.Map<BookingVM>(booking);

            return ChangeRoomResult.Success(
                bookingVm, 
                priceDifference, 
                "Đổi phòng thành công");
        }
        catch (Exception ex) when (ex is not DomainException and not NotFoundException)
        {
            return ChangeRoomResult.Failure($"Lỗi khi đổi phòng: {ex.Message}");
        }
    }

    /// <summary>
    /// Kiểm tra phòng đích có khả dụng không
    /// </summary>
    private async Task ValidateNewRoomAvailability(Booking booking, int newRoomId)
    {
        if (!booking.CheckInDateTime.HasValue || !booking.CheckOutDateTime.HasValue)
            throw new DomainException("Thời gian check-in và check-out không hợp lệ");

        // Kiểm tra conflict với booking khác
        var conflictBookings = _bookingRepository.GetQueryable()
            .Where(b => b.Id != booking.Id &&
                       b.Rooms!.Any(br => br.RoomId == newRoomId) &&
                       (b.Status == BookingStatus.Confirmed || 
                        b.Status == BookingStatus.CheckedIn) &&
                       b.CheckInDateTime < booking.CheckOutDateTime &&
                       b.CheckOutDateTime > booking.CheckInDateTime)
            .Any();

        if (conflictBookings)
        {
            var room = await _roomRepository.GetByIdAsync(newRoomId);
            throw new DomainException($"Phòng {room?.Name} đã được đặt trong cùng thời gian");
        }
    }

    /// <summary>
    /// Tính toán chênh lệch giá giữa phòng cũ và phòng mới
    /// </summary>
    private async Task<double> CalculatePriceDifference(Booking booking, int oldRoomId, int newRoomId)
    {
        try
        {
            var oldRoom = await _roomRepository.GetByIdAsync(oldRoomId);
            var newRoom = await _roomRepository.GetByIdAsync(newRoomId);

            if (oldRoom == null || newRoom == null)
                return 0;

            // Nếu cùng loại phòng thì không có chênh lệch
            if (oldRoom.Type == newRoom.Type)
                return 0;

            // Tạo booking clone để tính toán
            var tempBooking = new Booking
            {
                Type = booking.Type,
                CheckInDateTime = booking.CheckInDateTime,
                CheckOutDateTime = booking.CheckOutDateTime,
                Rooms = new List<BookingRoom>
                {
                    new() { Room = newRoom, RoomId = newRoomId }
                }
            };

            var newAmount = await _amountCalculator.CalculateAmount(tempBooking);

            tempBooking.Rooms = new List<BookingRoom>
            {
                new() { Room = oldRoom, RoomId = oldRoomId }
            };

            var oldAmount = await _amountCalculator.CalculateAmount(tempBooking);

            return newAmount.TotalAmount - oldAmount.TotalAmount;
        }
        catch
        {
            // Nếu có lỗi tính toán thì return 0
            return 0;
        }
    }
}
