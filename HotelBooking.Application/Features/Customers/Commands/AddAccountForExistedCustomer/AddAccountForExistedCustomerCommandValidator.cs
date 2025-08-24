using FluentValidation;

namespace HotelBooking.Application.Features.Customers.Commands.AddAccountForExistedCustomer;

public class AddAccountForExistedCustomerCommandValidator : AbstractValidator<AddAccountForExistedCustomerCommand>
{
    public AddAccountForExistedCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
