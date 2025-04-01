using AccommodationBookingPlatform.Application.Bookings.Common;
using AccommodationBookingPlatform.Application.Bookings.Create;
using AccommodationBookingPlatform.Application.Bookings.Delete;
using AccommodationBookingPlatform.Application.Bookings.GetById;
using AccommodationBookingPlatform.Application.Bookings.GetForGuest;
using AccommodationBookingPlatform.Application.Bookings.GetInvoiceAsPdf;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Bookings;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/user/bookings")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Guest)]
public class BookingsController(ISender mediator, IMapper mapper) : ControllerBase
{

    /// <response code="400">
    ///   If the request data is invalid or The provided rooms does not belong to the same
    ///   hotel or one of the rooms is not available in the specified times of check-in and check-out.
    /// </response>

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBooking(
      BookingCreationRequest bookingCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateBookingCommand>(bookingCreationRequest);

        var createdBooking = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
    }


    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBookingCommand { BookingId = id };

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}/invoice")]
    public async Task<FileResult> GetInvoiceAsPdf(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceAsPdfQuery { BookingId = id };

        var pdf = await mediator.Send(query, cancellationToken);

        return File(pdf, "application/pdf", "invoice.pdf");
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(
      Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBookingByIdQuery { BookingId = id };

        var booking = await mediator.Send(query, cancellationToken);

        return Ok(booking);
    }

    /// <summary>
    ///   Get a page of bookings for the current user based on the provided parameters.
    /// </summary>

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings(
      [FromQuery] BookingsGetRequest bookingsGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetBookingsQuery>(bookingsGetRequest);

        var bookings = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(bookings.PaginationMetadata);

        return Ok(bookings.Items);
    }
}