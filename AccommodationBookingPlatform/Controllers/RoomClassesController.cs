using AccommodationBookingPlatform.Application.RoomClasses.AddImageToGallery;
using AccommodationBookingPlatform.Application.RoomClasses.Create;
using AccommodationBookingPlatform.Application.RoomClasses.Delete;
using AccommodationBookingPlatform.Application.RoomClasses.GetForManagement;
using AccommodationBookingPlatform.Application.RoomClasses.Update;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Images;
using AccommodationBookingPlatform.DTOs.RoomClasses;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/room-classes")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Admin)]
public class RoomClassesController(ISender mediator, IMapper mapper) : ControllerBase
{
   
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomClassForManagementResponse>>> GetRoomClassesForManagement(
      [FromQuery] RoomClassesGetRequest roomClassesGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetRoomClassesForManagementQuery>(roomClassesGetRequest);

        var roomClasses = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(roomClasses.PaginationMetadata);

        return Ok(roomClasses.Items);
    }

  
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateRoomClass(
      RoomClassCreationRequest roomClassCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateRoomClassCommand>(roomClassCreationRequest);

        await mediator.Send(command, cancellationToken);

        return Created();
    }

    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRoomClass(
      Guid id,
      RoomClassUpdateRequest roomClassUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateRoomClassCommand { RoomClassId = id };
        mapper.Map(roomClassUpdateRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

  
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRoomClass(
      Guid id,
      CancellationToken cancellationToken)
    {
        var command = new DeleteRoomClassCommand { RoomClassId = id };

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("{id:guid}/gallery")]
    public async Task<IActionResult> AddImageToHotelGallery(
      Guid id,
      [FromForm] ImageCreationRequest imageCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = new AddImageToRoomClassGalleryCommand { RoomClassId = id };
        mapper.Map(imageCreationRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}