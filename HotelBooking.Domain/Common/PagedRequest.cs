namespace HotelBooking.Domain.Common;

public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchKey { get; set; }
    public string? OrderBy { get; set; }
    public bool IsDescending { get; set; }
}
