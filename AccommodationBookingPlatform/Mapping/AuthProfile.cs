using AccommodationBookingPlatform.DTOs.Auth;
using Application.Users.Login;
using Application.Users.Register;
using AutoMapper;


namespace AccommodationBookingPlatform.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<RegisterRequest, RegisterCommand>();
    }
}