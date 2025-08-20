using AutoMapper;
using HotelBooking.Application.Features.Rooms.DTOs;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Queries.GetRoomById;

public record GetRoomByIdQuery(int Id) : IRequest<RoomDTO?>;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, RoomDTO?>
{
    private readonly IMapper _mapper;
    private readonly IRoomRepository _repository;

    public GetRoomByIdQueryHandler(IMapper mapper, IRoomRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<RoomDTO?> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id);
        return room != null ? _mapper.Map<RoomDTO>(room) : null;
    }
}
