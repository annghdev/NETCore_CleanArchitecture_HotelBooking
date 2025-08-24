using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Auth.Login;

public record LoginCommand(
         AccountOrigin AccountOrigin,
         OpenPlatform Platform,
         string? Credential) : IRequest<AuthResponse>;

public enum OpenPlatform
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
        string schema = values[0];
        string identity = values[1];
        string password = values[2];
        return (schema, identity, password);
    }
}

