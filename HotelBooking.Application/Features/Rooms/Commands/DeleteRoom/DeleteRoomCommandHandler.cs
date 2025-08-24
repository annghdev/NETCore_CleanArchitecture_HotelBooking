using HotelBooking.Domain;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand>
{
    private readonly IRoomRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoomCommandHandler(IRoomRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Room), request.Id.ToString());
        
        await _repository.DeleteAsync(room);
        await _unitOfWork.SaveChangesAsync();
    }
}