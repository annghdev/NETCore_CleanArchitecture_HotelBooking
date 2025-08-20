using AutoMapper;
using HotelBooking.Domain;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand>
{
    private readonly IRoomRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomCommandHandler(IRoomRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var existingRoom = await _repository.GetByIdAsync(request.Room.Id) ?? throw new NotFoundException(nameof(Room), request.Room.Id.ToString());
        _mapper.Map(request.Room, existingRoom);
        await _repository.UpdateAsync(existingRoom);
        await _unitOfWork.SaveChangesAsync();
    }
}
