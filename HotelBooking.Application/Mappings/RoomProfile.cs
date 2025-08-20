using AutoMapper;
using HotelBooking.Application.Features.Rooms.DTOs;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<Room, RoomDTO>();
        CreateMap<CreateRoomDTO, Room>();
        CreateMap<UpdateRoomDTO, Room>();
    }
}
