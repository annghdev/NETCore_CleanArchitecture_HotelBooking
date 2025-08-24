using FluentValidation;
using HotelBooking.Application.Features.Auth.ConfirmEmail;

namespace HotelBooking.Application.Features.Auth.ConfirmAccount;

public class ConfirmAccountCommandValidator : AbstractValidator<ConfirmAccountCommand>
{
    public ConfirmAccountCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.ConfirmType)
            .IsInEnum()
            .WithMessage("Invalid confirmation type");

        RuleFor(x => x.Credential)
            .NotEmpty()
            .WithMessage("Confirmation credential is required")
            .MaximumLength(500)
            .WithMessage("Credential must not exceed 500 characters");

        // Validation riêng cho email
        When(x => x.ConfirmType == ConfirmType.ByEmail, () =>
        {
            RuleFor(x => x.Credential)
                .EmailAddress()
                .WithMessage("Invalid email format for email confirmation");
        });

        // Validation riêng cho phone
        When(x => x.ConfirmType == ConfirmType.ByPhoneNumber, () =>
        {
            RuleFor(x => x.Credential)
                .Must(BeValidPhoneNumber)
                .WithMessage("Invalid phone number format for phone confirmation");
        });
    }

    private static bool BeValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove common phone number formatting
        var cleanNumber = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        // Check if contains only digits and has valid length
        return cleanNumber.All(char.IsDigit) && cleanNumber.Length >= 10 && cleanNumber.Length <= 15;
    }
}
