using AutoMapper;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Application.Features.Rooms.Commands.CreateRoom;
using HotelBooking.Application.Features.Rooms.Commands.UpdateRoom;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class RoomMapping : Profile
{
    public RoomMapping()
    {
        CreateMap<Room, RoomVM>();
        CreateMap<CreateRoomCommand, Room>();
        CreateMap<UpdateRoomCommand, Room>();
    }
}
