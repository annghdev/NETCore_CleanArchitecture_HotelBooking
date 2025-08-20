using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Auth.Login;

public record LoginCommand(
         AccountOrigin AccountOrigin,
         Platform Platform,
         string? Credential) : IRequest;

public enum Platform
{
    Web,
    Mobile,
    DestopApp
}

public static class SplitPassword
{
    public static (string, string, string) Excute(string credential)
    {
        var values = credential.Split('|');
        string scheme = values[0];
        string identity = values[1];
        string password = values[2];
        return (scheme, identity, password);
    }
}

