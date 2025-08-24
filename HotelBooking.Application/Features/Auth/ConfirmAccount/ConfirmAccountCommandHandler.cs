using HotelBooking.Application.Features.Auth.ConfirmEmail;
using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Auth.ConfirmAccount;

public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmAccountCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        // Tìm user theo ID
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User", request.UserId.ToString());
        }

        // Kiểm tra user đã confirmed chưa
        if (user.IsConfirmed)
        {
            // Không throw exception, chỉ return success (idempotent)
            return;
        }

        // Validate credential theo type
        bool isValidCredential = request.ConfirmType switch
        {
            ConfirmType.ByEmail => ValidateEmailCredential(user, request.Credential),
            ConfirmType.ByPhoneNumber => ValidatePhoneCredential(user, request.Credential),
            _ => throw new ArgumentException($"Unsupported confirm type: {request.ConfirmType}")
        };

        if (!isValidCredential)
        {
            throw new InvalidOperationException("Invalid confirmation credential");
        }

        // Cập nhật trạng thái confirmed
        user.IsConfirmed = true;
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private static bool ValidateEmailCredential(Domain.Entities.User user, string credential)
    {
        // Trong thực tế, credential sẽ là confirmation code được gửi qua email
        // Hiện tại đơn giản hóa: credential phải match với email của user
        return !string.IsNullOrWhiteSpace(user.Email) && 
               string.Equals(user.Email, credential, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidatePhoneCredential(Domain.Entities.User user, string credential)
    {
        // Trong thực tế, credential sẽ là OTP được gửi qua SMS
        // Hiện tại đơn giản hóa: credential phải match với phone number của user
        return !string.IsNullOrWhiteSpace(user.PhoneNumber) && 
               user.PhoneNumber == credential;
    }
}
