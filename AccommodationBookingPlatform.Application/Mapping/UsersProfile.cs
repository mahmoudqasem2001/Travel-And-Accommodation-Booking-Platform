using AccommodationBookingPlatform.Domain.Entities;
using Application.Users.Login;
using Application.Users.Register;
using AutoMapper;
using Domain.Models;


namespace AccommodationBookingPlatform.Application.Mapping;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<JwtToken, LoginResponse>();
        CreateMap<RegisterCommand, User>();
    }
}