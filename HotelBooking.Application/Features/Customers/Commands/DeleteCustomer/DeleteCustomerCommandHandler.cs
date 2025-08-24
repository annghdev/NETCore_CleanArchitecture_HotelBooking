using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // Tìm customer theo ID
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        
        if (customer == null)
        {
            throw new NotFoundException("Customer", request.CustomerId.ToString());
        }

        // Kiểm tra xem customer có booking nào đang active không
        var activeBookings = _bookingRepository.GetQueryable()
            .Where(b => b.CustomerId == request.CustomerId 
                && (b.Status == Domain.Entities.BookingStatus.Confirmed 
                    || b.Status == Domain.Entities.BookingStatus.CheckedIn
                    || b.Status == Domain.Entities.BookingStatus.Pending))
            .Any();

        if (activeBookings)
        {
            throw new InvalidOperationException("Cannot delete customer with active bookings");
        }

        // Xóa customer (soft delete)
        await _customerRepository.DeleteAsync(customer);
        
        // Lưu changes
        await _unitOfWork.SaveChangesAsync();
    }
}
