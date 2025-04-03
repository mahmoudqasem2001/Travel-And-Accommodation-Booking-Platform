using AccommodationBookingPlatform.Application.Reviews.Common;
using AccommodationBookingPlatform.Application.Reviews.Create;
using AccommodationBookingPlatform.Application.Reviews.Delete;
using AccommodationBookingPlatform.Application.Reviews.GetByHotelId;
using AccommodationBookingPlatform.Application.Reviews.GetById;
using AccommodationBookingPlatform.Application.Reviews.Update;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Reviews;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/reviews")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Guest)]
public class ReviewsController(ISender mediator, IMapper mapper) : ControllerBase
{
   
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetReviewsForHotel(
      Guid hotelId,
      [FromQuery] ReviewsGetRequest reviewsGetRequest,
      CancellationToken cancellationToken)
    {
        var query = new GetReviewsByHotelIdQuery { HotelId = hotelId };
        mapper.Map(reviewsGetRequest, query);

        var reviews = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(reviews.PaginationMetadata);

        return Ok(reviews.Items);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewResponse>> GetReviewById(
      Guid hotelId, Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetReviewByIdQuery
        {
            HotelId = hotelId,
            ReviewId = id
        };

        var review = await mediator.Send(query, cancellationToken);

        return Ok(review);
    }

 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateReviewForHotel(Guid hotelId,
      ReviewCreationRequest reviewCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand { HotelId = hotelId };
        mapper.Map(reviewCreationRequest, command);

        var createdReview = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetReviewById), new { id = createdReview }, createdReview);
    }


    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateReviewForHotel(
      Guid hotelId, Guid id,
      ReviewUpdateRequest reviewUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateReviewCommand { HotelId = hotelId, ReviewId = id };
        mapper.Map(reviewUpdateRequest, command);

        await mediator.Send(
          command,
          cancellationToken);

        return NoContent();
    }

 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteReviewForHotel(
      Guid hotelId, Guid id,
      CancellationToken cancellationToken)
    {
        var command = new DeleteReviewCommand { HotelId = hotelId, ReviewId = id };

        await mediator.Send(
          command,
          cancellationToken);

        return NoContent();
    }
}