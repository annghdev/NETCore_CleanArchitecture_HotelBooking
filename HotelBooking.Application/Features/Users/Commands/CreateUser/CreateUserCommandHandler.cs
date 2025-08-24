using AutoMapper;
using HotelBooking.Domain;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserVM>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserVM> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra username đã tồn tại chưa
        var existingUser = await _userRepository.GetSingleAsync(u => u.UserName == request.UserName);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{request.UserName}' already exists");
        }

        // Kiểm tra email đã tồn tại chưa (nếu có)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingEmailUser = await _userRepository.GetSingleAsync(u => u.Email == request.Email);
            if (existingEmailUser != null)
            {
                throw new InvalidOperationException($"Email '{request.Email}' already exists");
            }
        }

        // Kiểm tra phone đã tồn tại chưa (nếu có)
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var existingPhoneUser = await _userRepository.GetSingleAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingPhoneUser != null)
            {
                throw new InvalidOperationException($"Phone number '{request.PhoneNumber}' already exists");
            }
        }

        // Tạo user mới
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Password = request.Password, // TODO: Hash password in real implementation
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AccountOrigin = request.AccountOrigin,
            IsConfirmed = false,
            LoginFailedCount = 0
        };

        var createdUser = await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserVM>(createdUser);
    }
}
