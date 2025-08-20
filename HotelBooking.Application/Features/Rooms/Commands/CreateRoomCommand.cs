using AutoMapper;
using HotelBooking.Application.Features.Rooms.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands;

public record CreateRoomCommand(CreateRoomDTO Room) : IRequest<RoomDTO>;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, RoomDTO>
{
    private readonly IRoomRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRoomCommandHandler(IRoomRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RoomDTO> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var newRoom = _mapper.Map<Room>(request.Room);
        await _repository.AddAsync(newRoom);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<RoomDTO>(newRoom);
    }
}
