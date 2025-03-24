using AccommodationBookingPlatform.Application.Rooms.Create;
using AccommodationBookingPlatform.Application.Rooms.Delete;
using AccommodationBookingPlatform.Application.Rooms.GetByRoomClassIdForGuest;
using AccommodationBookingPlatform.Application.Rooms.GetForManagement;
using AccommodationBookingPlatform.Application.Rooms.Update;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Rooms;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/room-classes/{roomClassId:guid}/rooms")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Admin)]
public class RoomsController(ISender mediator, IMapper mapper) : ControllerBase
{
  
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomForManagementResponse>>> GetRoomsForManagement(
      Guid roomClassId,
      [FromQuery] DTOs.Rooms.RoomsGetRequest roomsGetRequest,
      CancellationToken cancellationToken)
    {
        var query = new GetRoomsForManagementQuery { RoomClassId = roomClassId };
        mapper.Map(roomsGetRequest, query);

        var rooms = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(rooms.PaginationMetadata);

        return Ok(rooms.Items);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoomForGuestResponse>>> GetRoomsForGuests(
      Guid roomClassId,
      [FromQuery] RoomsForGuestsGetRequest roomsForGuestsGetRequest,
      CancellationToken cancellationToken)
    {
        var query = new GetRoomsByRoomClassIdForGuestsQuery { RoomClassId = roomClassId };
        mapper.Map(roomsForGuestsGetRequest, query);

        var rooms = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(rooms.PaginationMetadata);

        return Ok(rooms.Items);
    }

    
    /// <response code="409">If there is a room with the same number in the room class of the room class.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateRoomInRoomClass(
      Guid roomClassId,
      RoomCreationRequest roomCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = new CreateRoomCommand { RoomClassId = roomClassId };
        mapper.Map(roomCreationRequest, command);

        await mediator.Send(command, cancellationToken);

        return Created();
    }


    /// <response code="409">If there is a room with the same number in the room class of the room class.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRoomInRoomClass(
      Guid roomClassId, Guid id,
      RoomUpdateRequest roomUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateRoomCommand
        {
            RoomClassId = roomClassId,
            RoomId = id
        };
        mapper.Map(roomUpdateRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }


    /// <response code="409">If there are bookings to the room.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRoomInRoomClass(Guid roomClassId,
      Guid id,
      CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoomCommand
        {
            RoomClassId = roomClassId,
            RoomId = id
        };

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}