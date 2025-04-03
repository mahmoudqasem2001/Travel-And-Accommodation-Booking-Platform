using AccommodationBookingPlatform.Domain.Entities;
using Domain.Models;


namespace AccommodationBookingPlatform.Domain.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    JwtToken Generate(User user);
}