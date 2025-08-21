using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Repositories;

public interface IBusinessSettingRepository
{
    Task Update(BusinessSetting businessSetting);
    Task<BusinessSetting?> GetSetting();
}
