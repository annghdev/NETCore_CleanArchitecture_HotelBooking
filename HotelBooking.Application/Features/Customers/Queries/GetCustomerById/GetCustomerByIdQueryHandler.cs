using AutoMapper;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerVM>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CustomerVM> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        // Tìm customer theo ID
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        
        if (customer == null)
        {
            throw new NotFoundException("Customer", request.Id.ToString());
        }

        // Map sang CustomerVM
        return _mapper.Map<CustomerVM>(customer);
    }
}
