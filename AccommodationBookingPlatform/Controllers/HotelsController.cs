
using AccommodationBookingPlatform.Application.Hotels.AddToGallery;
using AccommodationBookingPlatform.Application.Hotels.Create;
using AccommodationBookingPlatform.Application.Hotels.Delete;
using AccommodationBookingPlatform.Application.Hotels.GetFeaturedDeals;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.GetForManagement;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.Search;
using AccommodationBookingPlatform.Application.Hotels.SetThumbnail;
using AccommodationBookingPlatform.Application.Hotels.Update;
using AccommodationBookingPlatform.Application.RoomClasses.GetByHotelIdForGuest;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Hotels;
using AccommodationBookingPlatform.DTOs.Images;
using AccommodationBookingPlatform.DTOs.RoomClasses;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/hotels")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Admin)]
public class HotelsController(ISender mediator, IMapper mapper) : ControllerBase
{

    [ProducesResponseType(typeof(IEnumerable<HotelForManagementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelForManagementResponse>>> GetHotelsForManagement(
      [FromQuery] HotelsGetRequest hotelsGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetHotelsForManagementQuery>(hotelsGetRequest);

        var hotels = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(hotels.PaginationMetadata);

        return Ok(hotels.Items);
    }


    [ProducesResponseType(typeof(IEnumerable<HotelSearchResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HotelSearchResultResponse>>> SearchAndFilterHotels(
      [FromQuery] HotelSearchRequest hotelSearchRequest,
      CancellationToken cancellationToken = default)
    {
        var query = mapper.Map<SearchForHotelsQuery>(hotelSearchRequest);

        var hotels = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(hotels.PaginationMetadata);

        return Ok(hotels.Items);
    }


    [ProducesResponseType(typeof(IEnumerable<HotelFeaturedDealResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("featured-deals")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HotelFeaturedDealResponse>>> GetFeaturedDeals(
      [FromQuery] HotelFeaturedDealsGetRequest hotelFeaturedDealsGetRequest,
      CancellationToken cancellationToken = default)
    {
        var query = mapper.Map<GetHotelFeaturedDealsQuery>(hotelFeaturedDealsGetRequest);

        var featuredDeals = await mediator.Send(query, cancellationToken);

        return Ok(featuredDeals);

    }
        [ProducesResponseType(typeof(HotelForGuestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<HotelForGuestResponse>> GetHotelForGuest(Guid id,
          CancellationToken cancellationToken = default)
        {
            var query = new GetHotelForGuestByIdQuery { HotelId = id };

            var hotel = await mediator.Send(query, cancellationToken);

            return Ok(hotel);
        }

    
    [ProducesResponseType(typeof(IEnumerable<RoomClassForGuestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}/room-classes")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoomClassForGuestResponse>>> GetRoomClassesForGuests(
      Guid id,
      [FromQuery] GetRoomClassesForGuestRequest getRoomClassesForGuestRequest,
      CancellationToken cancellationToken = default)
    {
        var query = new GetRoomClassesByHotelIdForGuestQuery { HotelId = id };
        mapper.Map(getRoomClassesForGuestRequest, query);

        var roomClasses = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(roomClasses.PaginationMetadata);

        return Ok(roomClasses.Items);
    }


    /// <response code="409">If there is an hotel in the same geographical location (longitude and latitude)</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateHotel(
      HotelCreationRequest hotelCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateHotelCommand>(hotelCreationRequest);

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
    public async Task<IActionResult> UpdateHotel(
      Guid id,
      HotelUpdateRequest hotelUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateHotelCommand { HotelId = id };
        mapper.Map(hotelUpdateRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }


    /// <response code="409">If there are room classes in the hotel.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHotel(
      Guid id,
      CancellationToken cancellationToken)
    {
        var command = new DeleteHotelCommand { HotelId = id };

        await mediator.Send(
          command,
          cancellationToken);

        return NoContent();
    }


    /// <response code="404">If the hotel specified by ID is not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/thumbnail")]
    public async Task<IActionResult> SetHotelThumbnail(
      Guid id,
      [FromForm] ImageCreationRequest imageCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = new SetHotelThumbnailCommand { HotelId = id };
        mapper.Map(imageCreationRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

 
    /// <response code="404">If the hotel specified by ID is not found.</response>
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
        var command = new AddImageToHotelGalleryCommand { HotelId = id };
        mapper.Map(imageCreationRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}