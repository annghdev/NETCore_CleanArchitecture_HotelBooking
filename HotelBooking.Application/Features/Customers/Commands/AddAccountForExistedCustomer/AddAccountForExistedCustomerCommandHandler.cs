using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.AddAccountForExistedCustomer;

public class AddAccountForExistedCustomerCommandHandler : IRequestHandler<AddAccountForExistedCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddAccountForExistedCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddAccountForExistedCustomerCommand request, CancellationToken cancellationToken)
    {
        // Tìm customer theo ID
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        
        if (customer == null)
        {
            throw new NotFoundException("Customer", request.CustomerId.ToString());
        }

        // Kiểm tra customer đã có account chưa
        if (customer.UserId.HasValue)
        {
            throw new InvalidOperationException("Customer already has an account");
        }

        // Tìm user theo ID
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            throw new NotFoundException("User", request.UserId.ToString());
        }

        // Kiểm tra user đã được liên kết với customer khác chưa
        var existingCustomer = _customerRepository.GetQueryable()
            .FirstOrDefault(c => c.UserId == request.UserId);

        if (existingCustomer != null)
        {
            throw new InvalidOperationException("User is already linked to another customer");
        }

        // Liên kết customer với user
        customer.UserId = request.UserId;

        // Cập nhật customer
        await _customerRepository.UpdateAsync(customer);
        
        // Lưu changes
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
