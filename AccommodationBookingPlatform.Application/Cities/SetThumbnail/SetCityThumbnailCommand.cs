﻿using MediatR;
using Microsoft.AspNetCore.Http;

namespace AccommodationBookingPlatform.Application.Cities.SetThumbnail;

public class SetCityThumbnailCommand : IRequest
{
    public Guid CityId { get; init; }
    public IFormFile Image { get; init; }
}