using HotelBooking.Domain;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.DeleteRoom;

public record DeleteRoomCommand(int Id) : IRequest;
