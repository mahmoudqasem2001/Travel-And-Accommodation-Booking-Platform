
using AccommodationBookingPlatform.Application.Amenities.Common;
using AccommodationBookingPlatform.Application.Amenities.Create;
using AccommodationBookingPlatform.Application.Amenities.Get;
using AccommodationBookingPlatform.Application.Amenities.GetById;
using AccommodationBookingPlatform.Application.Amenities.Update;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Amenities;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/amenities")]
[ApiVersion("1.0")]
public class AmenitiesController(ISender mediator, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AmenityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AmenityResponse>>> GetAmenities(
      [FromQuery] AmenitiesGetRequest amenitiesGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetAmenitiesQuery>(amenitiesGetRequest);

        var amenities = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(amenities.PaginationMetadata);

        return Ok(amenities.Items);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AmenityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmenityResponse>> GetAmenity(
      Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAmenityByIdQuery { AmenityId = id };

        var amenity = await mediator.Send(query, cancellationToken);

        return Ok(amenity);
    }


    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAmenity(
      AmenityCreationRequest amenityCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateAmenityCommand>(amenityCreationRequest);

        var createdAmenity = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAmenity), new { id = createdAmenity.Id }, createdAmenity);
    }


    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAmenity(
      Guid id,
      AmenityUpdateRequest amenityUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateAmenityCommand { AmenityId = id };
        mapper.Map(amenityUpdateRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}