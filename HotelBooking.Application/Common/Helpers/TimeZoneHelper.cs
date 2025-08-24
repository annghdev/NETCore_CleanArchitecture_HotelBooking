namespace HotelBooking.Application.Common.Helpers;

/// <summary>
/// Helper class để xử lý timezone Vietnam (UTC+7)
/// </summary>
public static class TimeZoneHelper
{
    /// <summary>
    /// Vietnam timezone offset: UTC+7
    /// </summary>
    public static readonly TimeSpan VietnamOffset = TimeSpan.FromHours(7);

    /// <summary>
    /// Vietnam TimeZoneInfo
    /// </summary>
    public static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone(
        "Vietnam Standard Time",
        VietnamOffset,
        "Vietnam Standard Time",
        "Vietnam Standard Time"
    );

    /// <summary>
    /// Convert Vietnam DateTimeOffset sang UTC cho database query
    /// </summary>
    /// <param name="vietnamTime">Thời gian Vietnam</param>
    /// <returns>Thời gian UTC</returns>
    public static DateTimeOffset ToUtc(DateTimeOffset vietnamTime)
    {
        // Nếu đã là UTC, return as-is
        if (vietnamTime.Offset == TimeSpan.Zero)
            return vietnamTime;

        // Nếu là Vietnam time, convert sang UTC
        if (vietnamTime.Offset == VietnamOffset)
            return vietnamTime.ToUniversalTime();

        // Assume input là Vietnam time nếu không rõ offset
        var vietnamDateTime = new DateTimeOffset(vietnamTime.DateTime, VietnamOffset);
        return vietnamDateTime.ToUniversalTime();
    }

    /// <summary>
    /// Convert UTC DateTimeOffset sang Vietnam time cho display
    /// </summary>
    /// <param name="utcTime">Thời gian UTC</param>
    /// <returns>Thời gian Vietnam</returns>
    public static DateTimeOffset ToVietnamTime(DateTimeOffset utcTime)
    {
        return utcTime.ToOffset(VietnamOffset);
    }

    /// <summary>
    /// Convert DateOnly sang Vietnam DateTimeOffset với specific time
    /// </summary>
    /// <param name="date">Ngày</param>
    /// <param name="time">Giờ (mặc định 00:00)</param>
    /// <returns>DateTimeOffset theo Vietnam timezone</returns>
    public static DateTimeOffset ToVietnamDateTimeOffset(DateOnly date, TimeOnly time = default)
    {
        var dateTime = date.ToDateTime(time);
        return new DateTimeOffset(dateTime, VietnamOffset);
    }

    /// <summary>
    /// Convert DateOnly sang UTC DateTimeOffset cho database query
    /// </summary>
    /// <param name="date">Ngày (assume Vietnam date)</param>
    /// <param name="time">Giờ (mặc định 00:00)</param>
    /// <returns>UTC DateTimeOffset</returns>
    public static DateTimeOffset DateOnlyToUtc(DateOnly date, TimeOnly time = default)
    {
        var vietnamTime = ToVietnamDateTimeOffset(date, time);
        return ToUtc(vietnamTime);
    }

    /// <summary>
    /// Tạo Vietnam day start (00:00 Vietnam time) -> UTC
    /// </summary>
    /// <param name="date">Vietnam date</param>
    /// <returns>UTC DateTimeOffset cho start of day</returns>
    public static DateTimeOffset GetVietnamDayStartUtc(DateTimeOffset vietnamDate)
    {
        var vietnamDateTime = vietnamDate.DateTime.Date; // Get date part only
        var vietnamDayStart = new DateTimeOffset(vietnamDateTime, VietnamOffset);
        return ToUtc(vietnamDayStart);
    }

    /// <summary>
    /// Tạo Vietnam day end (23:59:59.999 Vietnam time) -> UTC
    /// </summary>
    /// <param name="date">Vietnam date</param>
    /// <returns>UTC DateTimeOffset cho end of day</returns>
    public static DateTimeOffset GetVietnamDayEndUtc(DateTimeOffset vietnamDate)
    {
        var vietnamDateTime = vietnamDate.DateTime.Date.AddDays(1).AddTicks(-1); // End of day
        var vietnamDayEnd = new DateTimeOffset(vietnamDateTime, VietnamOffset);
        return ToUtc(vietnamDayEnd);
    }

    /// <summary>
    /// Tạo Vietnam day start từ DateOnly -> UTC
    /// </summary>
    /// <param name="date">Vietnam date</param>
    /// <returns>UTC DateTimeOffset cho start of day</returns>
    public static DateTimeOffset GetDateOnlyStartUtc(DateOnly date)
    {
        return DateOnlyToUtc(date, TimeOnly.MinValue);
    }

    /// <summary>
    /// Tạo Vietnam day end từ DateOnly -> UTC
    /// </summary>
    /// <param name="date">Vietnam date</param>
    /// <returns>UTC DateTimeOffset cho end of day</returns>
    public static DateTimeOffset GetDateOnlyEndUtc(DateOnly date)
    {
        return DateOnlyToUtc(date, TimeOnly.MaxValue);
    }

    /// <summary>
    /// Ensure DateTimeOffset có proper Vietnam timezone info
    /// Nếu input không có timezone info, assume Vietnam time
    /// </summary>
    /// <param name="input">Input DateTimeOffset</param>
    /// <returns>DateTimeOffset với proper Vietnam timezone</returns>
    public static DateTimeOffset EnsureVietnamTimeZone(DateTimeOffset input)
    {
        // Nếu đã có Vietnam offset, return as-is
        if (input.Offset == VietnamOffset)
            return input;

        // Nếu có offset khác UTC, convert về Vietnam
        if (input.Offset != TimeSpan.Zero)
        {
            return TimeZoneInfo.ConvertTime(input, VietnamTimeZone);
        }

        // Nếu UTC hoặc unspecified, assume là Vietnam time
        return new DateTimeOffset(input.DateTime, VietnamOffset);
    }

    /// <summary>
    /// Convert DateTime sang Vietnam DateTimeOffset (assume original is UTC from database)
    /// Used for AutoMapper conversions from Entity to ViewModel
    /// </summary>
    /// <param name="utcDateTime">UTC DateTime from database</param>
    /// <returns>Vietnam DateTimeOffset for display</returns>
    public static DateTimeOffset? ConvertUtcDateTimeToVietnamOffset(DateTime? utcDateTime)
    {
        if (!utcDateTime.HasValue)
            return null;

        var utcDateTimeOffset = new DateTimeOffset(utcDateTime.Value, TimeSpan.Zero);
        return ToVietnamTime(utcDateTimeOffset);
    }

    /// <summary>
    /// Convert DateTimeOffset sang Vietnam DateTimeOffset for display
    /// Used for AutoMapper conversions from Entity to ViewModel
    /// </summary>
    /// <param name="utcDateTimeOffset">UTC DateTimeOffset from database</param>
    /// <returns>Vietnam DateTimeOffset for display</returns>
    public static DateTimeOffset? ConvertUtcOffsetToVietnamOffset(DateTimeOffset? utcDateTimeOffset)
    {
        if (!utcDateTimeOffset.HasValue)
            return null;

        return ToVietnamTime(utcDateTimeOffset.Value);
    }

    /// <summary>
    /// Convert DateTimeOffset sang Vietnam DateTimeOffset for display (non-nullable version)
    /// </summary>
    /// <param name="utcDateTimeOffset">UTC DateTimeOffset from database</param>
    /// <returns>Vietnam DateTimeOffset for display</returns>
    public static DateTimeOffset ConvertUtcOffsetToVietnamOffset(DateTimeOffset utcDateTimeOffset)
    {
        return ToVietnamTime(utcDateTimeOffset);
    }
}
