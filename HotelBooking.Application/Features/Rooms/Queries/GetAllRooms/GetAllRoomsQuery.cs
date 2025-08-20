using AutoMapper;
using HotelBooking.Application.Features.Rooms.DTOs;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Queries.GetAllRooms;

public record GetAllRoomsQuery : IRequest<IEnumerable<RoomDTO>>;

public class GetAllRoomsQueryHandler : IRequestHandler<GetAllRoomsQuery, IEnumerable<RoomDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRoomRepository _repository;

    public GetAllRoomsQueryHandler(IMapper mapper, IRoomRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<IEnumerable<RoomDTO>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RoomDTO>>(rooms);
    }
}
