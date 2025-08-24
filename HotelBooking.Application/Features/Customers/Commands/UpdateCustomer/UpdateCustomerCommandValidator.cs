using FluentValidation;

namespace HotelBooking.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^[0-9+\-\s()]+$").WithMessage("Phone number format is invalid")
            .MinimumLength(10).WithMessage("Phone number must be at least 10 characters")
            .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters");

        RuleFor(x => x.IdentityNo)
            .MaximumLength(50).WithMessage("Identity number cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.IdentityNo));

        RuleFor(x => x.SessionId)
            .MaximumLength(100).WithMessage("Session ID cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SessionId));
    }
}
