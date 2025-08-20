using AutoMapper;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.CreateRoom;

public record CreateRoomCommand(
         string Name,
         int Floor,
         string? Description,
         RoomType Type,
         int Capacity,
         string? MainImageUrl = null,
         string? ImageUrls = null,
         bool IsActive = true) : IRequest<RoomVM>;


