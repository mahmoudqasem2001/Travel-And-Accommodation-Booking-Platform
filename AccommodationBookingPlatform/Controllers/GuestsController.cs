using System.Security.Claims;
using AccommodationBookingPlatform.Application.Hotels.GetRecentlyVisited;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Hotels;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/user")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Guest)]
public class GuestsController(ISender mediator, IMapper mapper) : ControllerBase
{
    
    [ProducesResponseType(typeof(IEnumerable<RecentlyVisitedHotelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet("recently-visited-hotels")]
    public async Task<ActionResult<IEnumerable<RecentlyVisitedHotelResponse>>> GetRecentlyVisitedHotels(
      [FromQuery] RecentlyVisitedHotelsGetRequest recentlyVisitedHotelsGetRequest,
      CancellationToken cancellationToken)
    {
        var guestId = Guid.Parse(
          User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? throw new ArgumentNullException());

        var query = new GetRecentlyVisitedHotelsForGuestQuery { GuestId = guestId };
        mapper.Map(recentlyVisitedHotelsGetRequest, query);

        var hotels = await mediator.Send(query, cancellationToken);

        return Ok(hotels);
    }
}