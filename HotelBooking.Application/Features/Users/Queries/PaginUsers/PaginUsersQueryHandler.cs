using AutoMapper;
using HotelBooking.Application.Common;
using HotelBooking.Domain.Common;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Users.Queries.PaginUsers;

public class PaginUsersQueryHandler : IRequestHandler<PaginUsersQuery, PagedResult<UserVM>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public PaginUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserVM>> Handle(PaginUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userRepository.GetQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            query = query.Where(u => u.UserName.Contains(request.UserName));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(u => u.FullName != null && u.FullName.Contains(request.Name));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(request.PhoneNumber));
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(u => u.Gender == request.Gender.Value);
        }

        if (request.IsConfirmed.HasValue)
        {
            query = query.Where(u => u.IsConfirmed == request.IsConfirmed.Value);
        }

        if (request.IsLocked.HasValue)
        {
            var isLocked = request.IsLocked.Value;
            var currentTimeUtc = DateTimeOffset.UtcNow; // Compare with UTC time in database
            if (isLocked)
            {
                query = query.Where(u => u.UnlockDate.HasValue && u.UnlockDate > currentTimeUtc);
            }
            else
            {
                query = query.Where(u => !u.UnlockDate.HasValue || u.UnlockDate <= currentTimeUtc);
            }
        }

        if (request.Origin.HasValue)
        {
            query = query.Where(u => u.AccountOrigin == request.Origin.Value);
        }

        // Apply date filters (convert Vietnam timezone to UTC for database comparison)
        if (request.CreatedDate.HasValue)
        {
            // Convert Vietnam timezone to UTC for database comparison
            var searchDateUtc = TimeZoneHelper.ToUtc(TimeZoneHelper.EnsureVietnamTimeZone(request.CreatedDate.Value));

            switch (request.SearchDateType)
            {
                case SearchDateType.After:
                    query = query.Where(u => u.CreatedDate >= searchDateUtc);
                    break;
                case SearchDateType.Before:
                    query = query.Where(u => u.CreatedDate <= searchDateUtc);
                    break;
                case SearchDateType.Equal:
                    // For Equal, compare date part only
                    var dayStartUtc = TimeZoneHelper.GetVietnamDayStartUtc(request.CreatedDate.Value);
                    var dayEndUtc = TimeZoneHelper.GetVietnamDayEndUtc(request.CreatedDate.Value);
                    query = query.Where(u => u.CreatedDate >= dayStartUtc && u.CreatedDate <= dayEndUtc);
                    break;
                case SearchDateType.InRange:
                    if (request.CreatedDateTo.HasValue)
                    {
                        var searchDateToUtc = TimeZoneHelper.ToUtc(TimeZoneHelper.EnsureVietnamTimeZone(request.CreatedDateTo.Value));
                        query = query.Where(u => u.CreatedDate >= searchDateUtc && u.CreatedDate <= searchDateToUtc);
                    }
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            query = query.Include(u => u.Roles).Where(u => u.Roles!.Any(r => r.Role!.Name.Contains(request.Role)));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            switch (request.OrderBy.ToLower())
            {
                case "username":
                    query = request.IsDescending
                        ? query.OrderByDescending(u => u.UserName)
                        : query.OrderBy(u => u.UserName);
                    break;
                case "fullname":
                    query = request.IsDescending
                        ? query.OrderByDescending(u => u.FullName)
                        : query.OrderBy(u => u.FullName);
                    break;
                case "email":
                    query = request.IsDescending
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email);
                    break;
                case "createddate":
                    query = request.IsDescending
                        ? query.OrderByDescending(u => u.CreatedDate)
                        : query.OrderBy(u => u.CreatedDate);
                    break;
                default:
                    query = query.OrderByDescending(u => u.CreatedDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedDate);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var skip = (request.PageNumber - 1) * request.PageSize;
        var users = await query
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userVMs = _mapper.Map<IEnumerable<UserVM>>(users);

        return new PagedResult<UserVM>(
            userVMs,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}
