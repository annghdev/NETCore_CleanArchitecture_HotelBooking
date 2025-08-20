using AutoMapper;
using HotelBooking.Domain;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, RoomVM>
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

    public async Task<RoomVM> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var newRoom = _mapper.Map<Room>(request.Room);
        await _repository.AddAsync(newRoom);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<RoomVM>(newRoom);
    }
}