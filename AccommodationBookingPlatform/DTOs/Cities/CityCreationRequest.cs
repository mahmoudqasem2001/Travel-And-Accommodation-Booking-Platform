﻿namespace AccommodationBookingPlatform.DTOs.Cities;

public class CityCreationRequest
{
    public string Name { get; init; }
    public string Country { get; init; }
    public string PostOffice { get; init; }
}