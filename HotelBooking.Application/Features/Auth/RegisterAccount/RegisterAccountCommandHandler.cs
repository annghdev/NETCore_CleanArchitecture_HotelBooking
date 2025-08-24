using AutoMapper;
using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Application.Features.Users;
using HotelBooking.Domain;

namespace HotelBooking.Application.Features.Auth.RegisterAccount;

public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public RegisterAccountCommandHandler(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Kiểm tra username đã tồn tại chưa
            var existingUserByUsername = await _userRepository.GetSingleAsync(u => u.UserName == request.UserName);
            if (existingUserByUsername != null)
            {
                return new AuthResponse(
                    Success: false,
                    AccessToken: null,
                    Message: "Username already exists");
            }

            // Kiểm tra email đã tồn tại chưa (nếu có)
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingUserByEmail = await _userRepository.GetSingleAsync(u => u.Email == request.Email);
                if (existingUserByEmail != null)
                {
                    return new AuthResponse(
                        Success: false,
                        AccessToken: null,
                        Message: "Email already exists");
                }
            }

            // Kiểm tra phone đã tồn tại chưa (nếu có)
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var existingUserByPhone = await _userRepository.GetSingleAsync(u => u.PhoneNumber == request.PhoneNumber);
                if (existingUserByPhone != null)
                {
                    return new AuthResponse(
                        Success: false,
                        AccessToken: null,
                        Message: "Phone number already exists");
                }
            }

            // Xử lý Customer
            Customer? customer = null;
            if (!request.IsNewCustomer && request.CustomerId.HasValue)
            {
                // Liên kết với customer có sẵn
                customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                {
                    return new AuthResponse(
                        Success: false,
                        AccessToken: null,
                        Message: "Customer not found");
                }

                // Kiểm tra customer đã có user chưa
                if (customer.UserId.HasValue)
                {
                    return new AuthResponse(
                        Success: false,
                        AccessToken: null,
                        Message: "Customer already has an account");
                }
            }

            // Tạo User mới
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                IsConfirmed = false, // Cần xác nhận email/phone
                AccountOrigin = AccountOrigin.System,
                LoginFailedCount = 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // Hash password
            newUser.Password = await _passwordHasher.GeneratePasswordHashAsync(newUser, request.Password);

            // Save User
            await _userRepository.AddAsync(newUser);

            // Tạo Customer mới nếu cần
            if (request.IsNewCustomer)
            {
                customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName ?? request.UserName,
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    IdentityNo = string.Empty, // Sẽ cập nhật sau
                    UserId = newUser.Id,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await _customerRepository.AddAsync(customer);
            }
            else if (customer != null)
            {
                // Liên kết customer có sẵn với user mới
                customer.UserId = newUser.Id;
                await _customerRepository.UpdateAsync(customer);
            }

            await _unitOfWork.SaveChangesAsync();

            // Generate tokens
            var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(newUser);
            var refreshToken = await _tokenGenerator.GenerateRefreshTokenAsync(newUser);

            // TODO: Save refresh token to database (cần UserTokenRepository)

            // Map user profile
            var userProfile = _mapper.Map<UserProfileVM>(newUser);

            return new AuthResponse(
                Success: true,
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                Message: "Account registered successfully. Please verify your email/phone.",
                Profile: userProfile);
        }
        catch (Exception ex)
        {
            return new AuthResponse(
                Success: false,
                AccessToken: null,
                Message: $"Registration failed: {ex.Message}");
        }
    }
}
