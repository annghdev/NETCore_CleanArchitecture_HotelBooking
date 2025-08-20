using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.DeleteRoom;

public record DeleteRoomCommand(int Id) : IRequest;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand>
{
    private readonly IRoomRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id) 
            ?? throw new NotFoundException(nameof(request), request.Id.ToString());
        await _repository.DeleteAsync(room);
        await _unitOfWork.SaveChangesAsync();
    }
}
