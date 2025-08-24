using AutoMapper;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerVM>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetAllCustomersQueryHandler(
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerVM>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        // Lấy tất cả customers
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        
        // Map sang CustomerVM
        return _mapper.Map<IEnumerable<CustomerVM>>(customers);
    }
}
