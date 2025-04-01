using AccommodationBookingPlatform.Application.Discounts.Create;
using AccommodationBookingPlatform.Application.Discounts.Delete;
using AccommodationBookingPlatform.Application.Discounts.Get;
using AccommodationBookingPlatform.Application.Discounts.GetById;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Discounts;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/room-classes/{roomClassId:guid}/discounts")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Admin)]
public class DiscountsController(ISender mediator, IMapper mapper) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DiscountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<DiscountResponse>>> GetAmenities(
      Guid roomClassId,
      [FromQuery] DiscountsGetRequest discountsGetRequest,
      CancellationToken cancellationToken)
    {
        var query = new GetDiscountsQuery { RoomClassId = roomClassId };
        mapper.Map(discountsGetRequest, query);

        var discounts = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(discounts.PaginationMetadata);

        return Ok(discounts.Items);
    }

  
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DiscountResponse>> GetDiscount(
      Guid roomClassId,
      Guid id,
      CancellationToken cancellationToken)
    {
        var query = new GetDiscountByIdQuery
        {
            RoomClassId = roomClassId,
            DiscountId = id
        };

        var discount = await mediator.Send(query,
          cancellationToken);

        return Ok(discount);
    }


    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateDiscount(
      Guid roomClassId,
      DiscountCreationRequest discountCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = new CreateDiscountCommand { RoomClassId = roomClassId };
        mapper.Map(discountCreationRequest, command);

        var createdDiscount = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetDiscount), new { id = createdDiscount.Id }, createdDiscount);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDiscount(
      Guid roomClassId,
      Guid id,
      CancellationToken cancellationToken)
    {
        var command = new DeleteDiscountCommand
        {
            RoomClassId = roomClassId,
            DiscountId = id
        };

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}