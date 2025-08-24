using AutoMapper;
using HotelBooking.Domain;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerVM>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerVM> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Map từ command sang entity
        var customer = _mapper.Map<Customer>(request);
        
        // Tạo customer mới
        var createdCustomer = await _customerRepository.AddAsync(customer);
        
        // Lưu changes
        await _unitOfWork.SaveChangesAsync();
        
        // Trả về CustomerVM
        return _mapper.Map<CustomerVM>(createdCustomer);
    }
}
