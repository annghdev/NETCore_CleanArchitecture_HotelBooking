using FluentValidation;

namespace HotelBooking.Application.Features.Auth.RegisterAccount;

public class RegisterAccountCommandValidator : AbstractValidator<RegisterAccountCommand>
{
    public RegisterAccountCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Username can only contain letters, numbers, underscore and dash");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters")
            .MaximumLength(100)
            .WithMessage("Password must not exceed 100 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email format is invalid");

        RuleFor(x => x.PhoneNumber)
            .Must(BeValidPhoneNumber)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number format is invalid");

        RuleFor(x => x.FullName)
            .Length(2, 100)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName))
            .WithMessage("Full name must be between 2 and 100 characters");

        // Business Rule: Email hoặc PhoneNumber phải có ít nhất 1 cái
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Either email or phone number is required");

        // Validation cho CustomerId khi không tạo customer mới
        When(x => !x.IsNewCustomer, () =>
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required when not creating a new customer");
        });

        // Validation cho FullName khi tạo customer mới
        When(x => x.IsNewCustomer, () =>
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required when creating a new customer")
                .Length(2, 100)
                .WithMessage("Full name must be between 2 and 100 characters");
        });
    }

    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return true; // Optional field

        // Remove common phone number formatting
        var cleanNumber = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        // Check if contains only digits and has valid length
        return cleanNumber.All(char.IsDigit) && cleanNumber.Length >= 10 && cleanNumber.Length <= 15;
    }
}
