using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.UpdateRoom;

public record UpdateRoomCommand(
         int Id,
         string Name,
         int Floor,
         string? Description,
         RoomType Type,
         int Capacity,
         string? MainImageUrl = null,
         string? ImageUrls = null,
         bool IsActive = true) : IRequest;

