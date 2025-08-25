namespace HotelBooking.Application.Features.Bookings.CalculateAmount;

public class AmountCalculator : IAmountCalculator
{
    private readonly IPricingPolicyRepository _pricingPolicyRepository;

    public AmountCalculator(IPricingPolicyRepository pricingPolicyRepository)
    {
        _pricingPolicyRepository = pricingPolicyRepository;
    }

    /// <summary>
    /// Tính toán chi tiết số tiền cho từng phòng dựa trên pricing policies
    /// </summary>
    public async Task<AmountResult> CalculateAmount(Booking booking)
    {
        var roomAmounts = new List<RoomAmount>();

        if (booking.Rooms == null || !booking.Rooms.Any())
        {
            throw new DomainException("Booking phải có ít nhất một phòng");
        }

        if (!booking.CheckInDateTime.HasValue || !booking.CheckOutDateTime.HasValue)
        {
            throw new DomainException("Booking phải có thời gian check-in và check-out");
        }

        // Lấy tất cả pricing policies một lần
        var allPolicies = await _pricingPolicyRepository.GetAllAsync();

        foreach (var bookingRoom in booking.Rooms)
        {
            if (bookingRoom.Room == null)
                continue;

            var roomAmount = await CalculateRoomAmount(
                bookingRoom.Room, 
                booking.Type, 
                booking.CheckInDateTime.Value, 
                booking.CheckOutDateTime.Value, 
                allPolicies);

            roomAmounts.Add(roomAmount);
        }

        return new AmountResult(roomAmounts);
    }

    /// <summary>
    /// Tính tiền cho một phòng cụ thể
    /// </summary>
    private async Task<RoomAmount> CalculateRoomAmount(
        Room room, 
        BookingType bookingType, 
        DateTimeOffset checkIn, 
        DateTimeOffset checkOut, 
        IEnumerable<PricingPolicy> allPolicies)
    {
        var details = new List<RoomAmountDetail>();
        
        // Tìm pricing policy phù hợp cho phòng và loại booking
        var applicablePolicies = allPolicies.Where(p => 
            p.RoomType == room.Type && 
            p.PricingType == bookingType).ToList();

        if (!applicablePolicies.Any())
        {
            throw new DomainException($"Không tìm thấy chính sách giá cho phòng {room.Name} và loại booking {bookingType}");
        }

        switch (bookingType)
        {
            case BookingType.Hourly:
                details.AddRange(await CalculateHourlyBooking(room, checkIn, checkOut, applicablePolicies));
                break;
            case BookingType.OverNight:
                details.AddRange(await CalculateOvernightBooking(room, checkIn, checkOut, applicablePolicies));
                break;
            case BookingType.Daily:
                details.AddRange(await CalculateDailyBooking(room, checkIn, checkOut, applicablePolicies));
                break;
            default:
                throw new DomainException($"Loại booking {bookingType} không được hỗ trợ");
        }

        return new RoomAmount(room.Name, room.Type.ToString(), details);
    }

    /// <summary>
    /// Tính tiền cho booking theo giờ
    /// </summary>
    private async Task<List<RoomAmountDetail>> CalculateHourlyBooking(
        Room room, 
        DateTimeOffset checkIn, 
        DateTimeOffset checkOut, 
        List<PricingPolicy> policies)
    {
        var details = new List<RoomAmountDetail>();
        var totalHours = (checkOut - checkIn).TotalHours;

        // Tìm policy phù hợp với thời gian
        var policy = GetApplicablePolicy(policies, checkIn, totalHours);
        
        if (policy == null)
        {
            throw new DomainException($"Không tìm thấy chính sách giá phù hợp cho booking {totalHours} giờ");
        }

        var baseAmount = policy.BasePrice;
        var extraHours = Math.Max(0, totalHours - (policy.MinDuration ?? 0));
        var extraAmount = extraHours * policy.ExtraUnitPrice;

        details.Add(new RoomAmountDetail(
            checkIn.ToString("dd/MM/yyyy"),
            GetDayOfWeekInVietnamese(checkIn.DayOfWeek),
            policy.Name,
            TimeOnly.FromTimeSpan(checkIn.TimeOfDay),
            TimeOnly.FromTimeSpan(checkOut.TimeOfDay),
            policy.BasePrice,
            baseAmount + extraAmount
        ));

        return details;
    }

    /// <summary>
    /// Tính tiền cho booking qua đêm
    /// </summary>
    private async Task<List<RoomAmountDetail>> CalculateOvernightBooking(
        Room room, 
        DateTimeOffset checkIn, 
        DateTimeOffset checkOut, 
        List<PricingPolicy> policies)
    {
        var details = new List<RoomAmountDetail>();

        // Overnight booking thường có giá cố định cho một đêm
        var policy = policies.FirstOrDefault();
        if (policy == null)
        {
            throw new DomainException("Không tìm thấy chính sách giá cho booking qua đêm");
        }

        details.Add(new RoomAmountDetail(
            checkIn.ToString("dd/MM/yyyy"),
            GetDayOfWeekInVietnamese(checkIn.DayOfWeek),
            policy.Name,
            TimeOnly.FromTimeSpan(checkIn.TimeOfDay),
            TimeOnly.FromTimeSpan(checkOut.TimeOfDay),
            policy.BasePrice,
            policy.BasePrice
        ));

        return details;
    }

    /// <summary>
    /// Tính tiền cho booking theo ngày
    /// </summary>
    private async Task<List<RoomAmountDetail>> CalculateDailyBooking(
        Room room, 
        DateTimeOffset checkIn, 
        DateTimeOffset checkOut, 
        List<PricingPolicy> policies)
    {
        var details = new List<RoomAmountDetail>();
        var current = checkIn.Date;
        var endDate = checkOut.Date;

        while (current < endDate)
        {
            var dayOfWeek = current.DayOfWeek;
            var policy = GetPolicyForDayOfWeek(policies, dayOfWeek);
            
            if (policy == null)
            {
                throw new DomainException($"Không tìm thấy chính sách giá cho {GetDayOfWeekInVietnamese(dayOfWeek)}");
            }

            details.Add(new RoomAmountDetail(
                current.ToString("dd/MM/yyyy"),
                GetDayOfWeekInVietnamese(dayOfWeek),
                policy.Name,
                policy.CheckInTimeDefault,
                policy.CheckOutTimeDefault,
                policy.BasePrice,
                policy.BasePrice
            ));

            current = current.AddDays(1);
        }

        return details;
    }

    /// <summary>
    /// Tìm policy phù hợp với thời gian booking
    /// </summary>
    private PricingPolicy? GetApplicablePolicy(List<PricingPolicy> policies, DateTimeOffset checkIn, double totalHours)
    {
        return policies.FirstOrDefault(p => 
            (p.MinDuration == null || totalHours >= p.MinDuration) &&
            (p.MaxDuration == null || totalHours <= p.MaxDuration) &&
            IsPolicyApplicableForDay(p, checkIn.DayOfWeek));
    }

    /// <summary>
    /// Tìm policy cho ngày trong tuần cụ thể
    /// </summary>
    private PricingPolicy? GetPolicyForDayOfWeek(List<PricingPolicy> policies, DayOfWeek dayOfWeek)
    {
        return policies.FirstOrDefault(p => IsPolicyApplicableForDay(p, dayOfWeek));
    }

    /// <summary>
    /// Kiểm tra policy có áp dụng cho ngày trong tuần không
    /// </summary>
    private bool IsPolicyApplicableForDay(PricingPolicy policy, DayOfWeek dayOfWeek)
    {
        if (string.IsNullOrEmpty(policy.DaysOfWeek))
            return true; // Áp dụng cho tất cả các ngày

        // Format: "1,2,3,4,5,6,0" (Monday=1, Sunday=0)
        var dayValue = dayOfWeek == DayOfWeek.Sunday ? 0 : (int)dayOfWeek;
        return policy.DaysOfWeek.Split(',').Contains(dayValue.ToString());
    }

    /// <summary>
    /// Chuyển đổi DayOfWeek sang tiếng Việt
    /// </summary>
    private string GetDayOfWeekInVietnamese(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ Hai",
            DayOfWeek.Tuesday => "Thứ Ba", 
            DayOfWeek.Wednesday => "Thứ Tư",
            DayOfWeek.Thursday => "Thứ Năm",
            DayOfWeek.Friday => "Thứ Sáu",
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ Nhật",
            _ => dayOfWeek.ToString()
        };
    }
}
