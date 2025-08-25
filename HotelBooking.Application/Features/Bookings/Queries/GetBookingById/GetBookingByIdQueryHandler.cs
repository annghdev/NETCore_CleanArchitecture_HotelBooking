using AutoMapper;

namespace HotelBooking.Application.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingVM>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBookingByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy thông tin chi tiết booking theo ID
    /// </summary>
    public async Task<BookingVM> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        // Lấy booking với các thông tin liên quan
        var booking = _unitOfWork.BookingRepository.GetQueryable()
            .Where(b => b.Id == request.Id)
            .Include(b => b.Customer)
            .Include(b => b.Rooms!)
                .ThenInclude(br => br.Room)
            .Include(b => b.Transactions)
            .FirstOrDefault();

        if (booking == null)
            throw new NotFoundException($"Không tìm thấy booking với ID: {request.Id}");

        // Convert entity sang ViewModel với timezone conversion
        var bookingVm = _mapper.Map<BookingVM>(booking);

        // Bổ sung thông tin room names cho BookingVM
        if (booking.Rooms?.Any() == true)
        {
            bookingVm.RoomId = booking.Rooms.First().RoomId;
            bookingVm.RoomName = string.Join(", ", booking.Rooms.Select(br => br.Room?.Name ?? ""));
        }

        // Map BookingRooms
        bookingVm.Rooms = booking.Rooms?.Select(br => new BookingRoomVM
        {
            Id = br.Id,
            RoomId = br.RoomId,
            RoomName = br.Room?.Name,
            SubTotal = br.SubTotal,
            Notes = br.Notes,
            ChangedToRoomId = br.ChangedToRoomId,
            ChangedToRoom = br.ChangedToRoom?.Room?.Name,
            ChangedRoomDate = TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(br.ChangedRoomDate)
        }) ?? [];

        // Map PaymentTransactions
        bookingVm.Transactions = booking.Transactions?.Select(t => new PaymentTransactionVM
        {
            Id = t.Id,
            BookingId = t.BookingId,
            Amount = t.Amount,
            Type = t.Type,
            TypeName = t.Type.ToString(),
            Origin = t.Origin,
            OriginName = t.Origin.ToString(),
            ProcessStatus = t.ProcessStatus,
            ProcessStatusName = t.ProcessStatus.ToString(),
            TransactionNo = t.TransactionNo,
            OccuredDate = TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(t.OccuredDate)
        }) ?? [];

        return bookingVm;
    }
}
