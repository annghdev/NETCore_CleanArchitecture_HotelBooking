using AutoMapper;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, IEnumerable<UserVM>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUserQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserVM>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserVM>>(users);
    }
}
