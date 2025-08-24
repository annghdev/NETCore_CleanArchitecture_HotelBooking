using FluentValidation;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0");

        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Room name is required")
            .MaximumLength(50).WithMessage("Room name cannot exceed 50 characters");

        RuleFor(r => r.Floor)
            .GreaterThan(0).WithMessage("Floor must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Floor cannot exceed 100");

        RuleFor(r => r.Type)
            .IsInEnum().WithMessage("Invalid room type");

        RuleFor(r => r.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Capacity cannot exceed 20 people");

        RuleFor(r => r.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(r => !string.IsNullOrEmpty(r.Description));

        RuleFor(r => r.MainImageUrl)
            .Must(BeValidUrl).WithMessage("Main image URL is not valid")
            .When(r => !string.IsNullOrEmpty(r.MainImageUrl));

        RuleFor(r => r.ImageUrls)
            .MaximumLength(2000).WithMessage("Image URLs cannot exceed 2000 characters")
            .When(r => !string.IsNullOrEmpty(r.ImageUrls));
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
