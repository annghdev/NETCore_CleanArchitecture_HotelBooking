using FluentValidation;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Auth.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.AccountOrigin)
            .NotEqual(AccountOrigin.Facebook)
            .WithMessage("Facebook authentication is not implemented yet");

        RuleFor(x => x.Platform)
            .IsInEnum()
            .WithMessage("Invalid platform specified");

        RuleFor(x => x.Credential)
            .NotEmpty()
            .WithMessage("Credential is required")
            .Must(BeValidCredentialFormat)
            .WithMessage("Credential must be in format: schema|identity|password");

        // Validation riêng cho từng AccountOrigin
        When(x => x.AccountOrigin == AccountOrigin.System, () =>
        {
            RuleFor(x => x.Credential)
                .Must(BeValidSystemCredential)
                .WithMessage("System credential must be: email|{email}|{password}, username|{username}|{password}, or phone|{phone}|{password}");
        });

        When(x => x.AccountOrigin == AccountOrigin.Google, () =>
        {
            RuleFor(x => x.Credential)
                .Must(BeValidGoogleCredential)
                .WithMessage("Google credential must be: google|{userinfo_json}|empty");
        });
    }

    private static bool BeValidCredentialFormat(string? credential)
    {
        if (string.IsNullOrWhiteSpace(credential))
            return false;

        var parts = credential.Split('|');
        return parts.Length == 3;
    }

    private static bool BeValidSystemCredential(string? credential)
    {
        if (!BeValidCredentialFormat(credential))
            return false;

        var (schema, identity, password) = SplitPassword.Excute(credential!);
        
        // Schema phải là email, username, hoặc phone
        if (schema != "email" && schema != "username" && schema != "phone")
            return false;

        // Identity và password không được empty
        if (string.IsNullOrWhiteSpace(identity) || string.IsNullOrWhiteSpace(password))
            return false;

        // Validate format theo schema
        return schema switch
        {
            "email" => IsValidEmail(identity),
            "phone" => IsValidPhoneNumber(identity),
            "username" => IsValidUsername(identity),
            _ => false
        };
    }

    private static bool BeValidGoogleCredential(string? credential)
    {
        if (!BeValidCredentialFormat(credential))
            return false;

        var (schema, userInfo, _) = SplitPassword.Excute(credential!);
        
        return schema == "google" && !string.IsNullOrWhiteSpace(userInfo);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Đơn giản check: chỉ chứa số và dấu +, có độ dài 10-15 ký tự
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var cleanNumber = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "");
        return cleanNumber.All(char.IsDigit) && cleanNumber.Length >= 10 && cleanNumber.Length <= 15;
    }

    private static bool IsValidUsername(string username)
    {
        // Username: 3-50 ký tự, chỉ chứa chữ, số, underscore, dash
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return username.Length >= 3 && 
               username.Length <= 50 && 
               username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }
}
