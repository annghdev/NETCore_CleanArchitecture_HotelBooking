using AutoMapper;
using HotelBooking.Application.Common.Utils;

namespace HotelBooking.Application.Features.Bookings.Queries.PaginBookings;

public class PaginBookingsCommandHandler : IRequestHandler<PaginBookingsCommand, PagedResult<BookingVM>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaginBookingsCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách booking có phân trang và lọc
    /// </summary>
    public async Task<PagedResult<BookingVM>> Handle(PaginBookingsCommand request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.BookingRepository.GetQueryable()
            .Include(b => b.Customer)
            .Include(b => b.Rooms!)
                .ThenInclude(br => br.Room)
            .Include(b => b.Transactions)
            .AsQueryable();

        // Áp dụng filters
        query = ApplyFilters(query, request);

        // Sắp xếp theo ngày tạo mới nhất
        query = query.OrderByDescending(b => b.CreatedDate);

        // Thực hiện phân trang
        var totalCount = query.Count();
        var bookings = query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToList();

        // Convert sang ViewModel
        var bookingVMs = bookings.Select(booking =>
        {
            var vm = _mapper.Map<BookingVM>(booking);
            
            // Bổ sung thông tin room names
            if (booking.Rooms?.Any() == true)
            {
                vm.RoomId = booking.Rooms.First().RoomId;
                vm.RoomName = string.Join(", ", booking.Rooms.Select(br => br.Room?.Name ?? ""));
            }

            // Map BookingRooms
            vm.Rooms = booking.Rooms?.Select(br => new BookingRoomVM
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
            vm.Transactions = booking.Transactions?.Select(t => new PaymentTransactionVM
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

            return vm;
        }).ToList();

        return new PagedResult<BookingVM>
        {
            Data = bookingVMs,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Áp dụng các bộ lọc cho query
    /// </summary>
    private IQueryable<Booking> ApplyFilters(IQueryable<Booking> query, PaginBookingsCommand request)
    {
        // Lọc theo tên khách hàng
        if (!string.IsNullOrEmpty(request.CustomerName))
        {
            query = query.Where(b => 
                (b.Customer != null && b.Customer.FullName.Contains(request.CustomerName)) ||
                (b.CustomerName != null && b.CustomerName.Contains(request.CustomerName)));
        }

        // Lọc theo số điện thoại
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            query = query.Where(b => 
                (b.Customer != null && b.Customer.PhoneNumber.Contains(request.PhoneNumber)) ||
                (b.PhoneNumber != null && b.PhoneNumber.Contains(request.PhoneNumber)));
        }

        // Lọc theo ngày tạo
        if (request.CreatedDate.HasValue)
        {
            var searchDate = TimeZoneHelper.ConvertVietnamOffsetToUtcOffset(request.CreatedDate.Value);
            
            switch (request.SearchDateType)
            {
                case SearchDateType.After:
                    query = query.Where(b => b.CreatedDate >= searchDate);
                    break;
                case SearchDateType.Before:
                    query = query.Where(b => b.CreatedDate <= searchDate);
                    break;
                case SearchDateType.OnDate:
                    var startOfDay = searchDate.Date;
                    var endOfDay = startOfDay.AddDays(1);
                    query = query.Where(b => b.CreatedDate >= startOfDay && b.CreatedDate < endOfDay);
                    break;
                case SearchDateType.Between:
                    if (request.CreatedDateTo.HasValue)
                    {
                        var endDate = TimeZoneHelper.ConvertVietnamOffsetToUtcOffset(request.CreatedDateTo.Value);
                        query = query.Where(b => b.CreatedDate >= searchDate && b.CreatedDate <= endDate);
                    }
                    break;
            }
        }

        return query;
    }
}
