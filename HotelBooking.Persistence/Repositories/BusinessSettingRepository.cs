using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class BusinessSettingRepository(BookingDbContext dbContext) : IBusinessSettingRepository
{
    private readonly BookingDbContext _dbContext = dbContext;

    public Task<BusinessSetting?> GetSetting()
    {
        return _dbContext.BusinessSettings.SingleOrDefaultAsync();
    }

    public Task Update(BusinessSetting businessSetting)
    {
        _dbContext.BusinessSettings.Update(businessSetting);
        return Task.CompletedTask;
    }
}
