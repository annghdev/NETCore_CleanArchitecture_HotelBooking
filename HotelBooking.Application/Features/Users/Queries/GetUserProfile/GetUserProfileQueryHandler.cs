using AutoMapper;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileVM>
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUserRepository userRepository, ICustomerRepository customerRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileVM> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // Tìm user
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new NotFoundException("User", request.Id.ToString());
        }

        // Tìm customer liên kết (nếu có)
        var customer = await _customerRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.UserId == request.Id, cancellationToken);

        UserProfileVM profile = _mapper.Map<UserProfileVM>(user);
        profile.Roles = [.. user.Roles!.Select(r => r.Role!.Name)];

        if (customer != null)
        {
            _mapper.Map(customer, profile);
        }

        return profile;
    }
}
