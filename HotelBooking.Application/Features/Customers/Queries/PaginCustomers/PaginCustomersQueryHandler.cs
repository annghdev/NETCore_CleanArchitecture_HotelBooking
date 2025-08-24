using AutoMapper;
using HotelBooking.Application.Common;
using HotelBooking.Domain.Common;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Customers.Queries.PaginCustomers;

public class PaginCustomersQueryHandler : IRequestHandler<PaginCustomersQuery, PagedResult<CustomerVM>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public PaginCustomersQueryHandler(
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<CustomerVM>> Handle(PaginCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = _customerRepository.GetQueryable();

        // Filter by full name
        if (!string.IsNullOrEmpty(request.FullName))
        {
            query = query.Where(c => c.FullName.Contains(request.FullName));
        }

        // Filter by phone number
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            query = query.Where(c => c.PhoneNumber.Contains(request.PhoneNumber));
        }

        // Filter by search term (tìm kiếm trong cả tên và phone)
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => 
                c.FullName.Contains(request.SearchTerm) || 
                c.PhoneNumber.Contains(request.SearchTerm) ||
                (c.IdentityNo != null && c.IdentityNo.Contains(request.SearchTerm)));
        }

        // Filter by created date (convert Vietnam timezone to UTC for database comparison)
        if (request.CreatedDate.HasValue)
        {
            // Convert Vietnam timezone to UTC for database comparison
            var searchDateUtc = TimeZoneHelper.ToUtc(TimeZoneHelper.EnsureVietnamTimeZone(request.CreatedDate.Value));
            
            switch (request.SearchDateType)
            {
                case SearchDateType.After:
                    query = query.Where(c => c.CreatedDate >= searchDateUtc);
                    break;
                case SearchDateType.Before:
                    query = query.Where(c => c.CreatedDate <= searchDateUtc);
                    break;
                case SearchDateType.Equal:
                    // For Equal, compare date part only
                    var dayStartUtc = TimeZoneHelper.GetVietnamDayStartUtc(request.CreatedDate.Value);
                    var dayEndUtc = TimeZoneHelper.GetVietnamDayEndUtc(request.CreatedDate.Value);
                    query = query.Where(c => c.CreatedDate >= dayStartUtc && c.CreatedDate <= dayEndUtc);
                    break;
                case SearchDateType.InRange:
                    if (request.CreatedDateTo.HasValue)
                    {
                        var searchDateToUtc = TimeZoneHelper.ToUtc(TimeZoneHelper.EnsureVietnamTimeZone(request.CreatedDateTo.Value));
                        query = query.Where(c => c.CreatedDate >= searchDateUtc && c.CreatedDate <= searchDateToUtc);
                    }
                    break;
            }
        }

        // Sorting
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            switch (request.OrderBy.ToLower())
            {
                case "fullname":
                    query = request.IsDescending 
                        ? query.OrderByDescending(c => c.FullName)
                        : query.OrderBy(c => c.FullName);
                    break;
                case "phonenumber":
                    query = request.IsDescending 
                        ? query.OrderByDescending(c => c.PhoneNumber)
                        : query.OrderBy(c => c.PhoneNumber);
                    break;
                case "createddate":
                    query = request.IsDescending 
                        ? query.OrderByDescending(c => c.CreatedDate)
                        : query.OrderBy(c => c.CreatedDate);
                    break;
                default:
                    query = query.OrderByDescending(c => c.CreatedDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(c => c.CreatedDate);
        }

        // Count total items
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var customers = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to CustomerVM
        var customerVMs = _mapper.Map<IEnumerable<CustomerVM>>(customers);

        return new PagedResult<CustomerVM>(customerVMs, totalCount, request.PageNumber, request.PageSize);
    }
}
