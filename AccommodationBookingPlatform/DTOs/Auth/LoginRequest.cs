﻿namespace AccommodationBookingPlatform.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; init; }
    public string Password { get; init; }
}