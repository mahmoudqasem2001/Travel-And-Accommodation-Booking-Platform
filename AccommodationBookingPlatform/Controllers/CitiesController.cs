using AccommodationBookingPlatform.Application.Cities.Create;
using AccommodationBookingPlatform.Application.Cities.Delete;
using AccommodationBookingPlatform.Application.Cities.GetForManagement;
using AccommodationBookingPlatform.Application.Cities.GetTrending;
using AccommodationBookingPlatform.Application.Cities.SetThumbnail;
using AccommodationBookingPlatform.Application.Cities.Update;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.DTOs.Cities;
using AccommodationBookingPlatform.DTOs.Images;
using AccommodationBookingPlatform.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AccommodationBookingPlatform.Controllers;

[ApiController]
[Route("api/cities")]
[ApiVersion("1.0")]
[Authorize(Roles = UserRoles.Admin)]
public class CitiesController(ISender mediator, IMapper mapper) : ControllerBase
{
 
    [ProducesResponseType(typeof(IEnumerable<CityForManagementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityForManagementResponse>>> GetCitiesForManagement(
      [FromQuery] CitiesGetRequest citiesGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetCitiesForManagementQuery>(citiesGetRequest);

        var cities = await mediator.Send(query, cancellationToken);

        Response.Headers.AddPaginationMetadata(cities.PaginationMetadata);

        return Ok(cities.Items);
    }

 
    [ProducesResponseType(typeof(IEnumerable<TrendingCityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("trending")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TrendingCityResponse>>> GetTrendingCities(
      [FromQuery] TrendingCitiesGetRequest trendingCitiesGetRequest,
      CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetTrendingCitiesQuery>(trendingCitiesGetRequest);

        var cities = await mediator.Send(query, cancellationToken);

        return Ok(cities);
    }

   
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateCity(
      CityCreationRequest cityCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateCityCommand>(cityCreationRequest);

        await mediator.Send(command, cancellationToken);

        return Created();
    }

 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCity(Guid id,
      CityUpdateRequest cityUpdateRequest,
      CancellationToken cancellationToken)
    {
        var command = new UpdateCityCommand { CityId = id };
        mapper.Map(cityUpdateRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCity(
      Guid id,
      CancellationToken cancellationToken)
    {
        var command = new DeleteCityCommand { CityId = id };

        await mediator.Send(
          command,
          cancellationToken);

        return NoContent();
    }


    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}/thumbnail")]
    public async Task<IActionResult> SetCityThumbnail(
      Guid id,
      [FromForm] ImageCreationRequest imageCreationRequest,
      CancellationToken cancellationToken)
    {
        var command = new SetCityThumbnailCommand { CityId = id };
        mapper.Map(imageCreationRequest, command);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}