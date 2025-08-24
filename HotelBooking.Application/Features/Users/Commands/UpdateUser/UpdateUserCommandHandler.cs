using AutoMapper;
using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // Tìm user cần update
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new NotFoundException("User", request.Id.ToString());
        }

        // Kiểm tra username đã tồn tại chưa (ngoại trừ user hiện tại)
        var existingUser = await _userRepository.GetSingleAsync(u => u.UserName == request.UserName && u.Id != request.Id);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{request.UserName}' already exists");
        }

        // Kiểm tra email đã tồn tại chưa (ngoại trừ user hiện tại)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingEmailUser = await _userRepository.GetSingleAsync(u => u.Email == request.Email && u.Id != request.Id);
            if (existingEmailUser != null)
            {
                throw new InvalidOperationException($"Email '{request.Email}' already exists");
            }
        }

        // Kiểm tra phone đã tồn tại chưa (ngoại trừ user hiện tại)
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var existingPhoneUser = await _userRepository.GetSingleAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != request.Id);
            if (existingPhoneUser != null)
            {
                throw new InvalidOperationException($"Phone number '{request.PhoneNumber}' already exists");
            }
        }

        // Cập nhật thông tin user
        user.UserName = request.UserName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.AccountOrigin = request.AccountOrigin;

        // Cập nhật password nếu có
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = request.Password; // TODO: Hash password in real implementation
        }

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
