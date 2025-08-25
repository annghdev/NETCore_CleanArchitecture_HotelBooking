namespace HotelBooking.Application.Common.Helpers;

public static class TransactionCodeGenerator
{
    public static string Generate(string prefix = "")
    {
        return $"{prefix}{DateTimeOffset.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
