using AccommodationBookingPlatform.Application.Owners.Common;
using AccommodationBookingPlatform.Application.Owners.Create;
using AccommodationBookingPlatform.Application.Owners.Get;
using AccommodationBookingPlatform.Application.Owners.GetById;
using AccommodationBookingPlatform.Application.Owners.Update;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Owners;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/owners")]
[Authorize(Roles = UserRoles.Admin)]
[ApiVersion("1.0")]
public class OwnersController(ISender mediator, IMapper mapper) : ControllerBase
{
   
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OwnerResponse>>> GetOwners(
      [FromQuery] OwnersGetRequest ownersGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetOwnersQuery>(ownersGetRequest);

        var owners = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(owners.PaginationMetadata);

        return Ok(owners.Items);
    }

   
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OwnerResponse>> GetOwner(Guid id,
      CancellationToken cancellationToken)
    {
        var query = new GetOwnerByIdQuery { OwnerId = id };

        var owner = await mediator.Send(query, cancellationToken);

        return Ok(owner);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    public async Task<IActionResult> CreateOwner(
      OwnerCreationRequest ownerCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateOwnerCommand>(ownerCreationRequest);

        var createdOwner = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetOwner), new { id = createdOwner.Id }, createdOwner);
    }

   
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOwner(Guid id, OwnerUpdateRequest ownerUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateOwnerCommand { OwnerId = id };
        mapper.Map(ownerUpdateRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}