using AutoMapper;
using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Tìm customer theo ID
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        
        if (customer == null)
        {
            throw new NotFoundException("Customer", request.CustomerId.ToString());
        }

        // Cập nhật thông tin customer
        customer.FullName = request.Name;
        customer.PhoneNumber = request.PhoneNumber;
        customer.IdentityNo = request.IdentityNo;
        customer.SessionId = request.SessionId;

        // Cập nhật customer
        await _customerRepository.UpdateAsync(customer);
        
        // Lưu changes
        await _unitOfWork.SaveChangesAsync();
    }
}
