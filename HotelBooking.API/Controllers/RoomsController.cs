using HotelBooking.Application.Features.Rooms.Commands.CreateRoom;
using HotelBooking.Application.Features.Rooms.Commands.DeleteRoom;
using HotelBooking.Application.Features.Rooms.Commands.UpdateRoom;
using HotelBooking.Application.Features.Rooms.Queries.GetAllRooms;
using HotelBooking.Application.Features.Rooms.Queries.GetRoomById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var result = await _mediator.Send(new GetAllRoomsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _mediator.Send(new GetRoomByIdQuery(id));
            return room != null ? Ok(room) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomDTO room)
        {
            var newRoom = await _mediator.Send(new CreateRoomCommand(room));
            return newRoom.Id > 0 ? CreatedAtAction("Create", newRoom) : StatusCode(500);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDTO room)
        {
            if (id != room.Id)
                return BadRequest();
            await _mediator.Send(new UpdateRoomCommand(room));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteRoomCommand(id));
            return NoContent();
        }
    }
}
