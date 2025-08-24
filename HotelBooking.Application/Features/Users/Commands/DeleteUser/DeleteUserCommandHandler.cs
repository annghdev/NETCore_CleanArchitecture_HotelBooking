using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // Tìm user cần xóa
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User", request.UserId.ToString());
        }

        // Kiểm tra user có liên kết với customer không
        var relatedCustomers = await _customerRepository.GetQueryable()
            .Where(c => c.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        if (relatedCustomers.Any())
        {
            throw new InvalidOperationException(
                $"Cannot delete user '{user.UserName}' because it is linked to {relatedCustomers.Count} customer(s). " +
                "Please unlink or delete related customers first.");
        }

        // Xóa user (soft delete sẽ được xử lý bởi AuditableEntity)
        await _userRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
