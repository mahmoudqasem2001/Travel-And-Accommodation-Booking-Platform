using AutoMapper;
using MediatR;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Domain.Interfaces.Auth;
using Application.Users.Login;
using AccommodationBookingPlatform.Domain.Entities;
using Domain.Exceptions;
using Domain.Messages;

namespace AccommodationBookingPlatform.Application.Users.Login;

public class LoginCommandHandler(
  IUserRepository userRepository,
  IJwtTokenGenerator jwtTokenGenerator,
  IMapper mapper) : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<LoginResponse> Handle(
    LoginCommand request,
    CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.AuthenticateAsync(request.Email,
                     request.Password, cancellationToken)
                   ?? throw new CredentialsNotValidException(UserMessages.CredentialsNotValid);
        //var user = new User();
        return _mapper.Map<LoginResponse>(_jwtTokenGenerator.Generate(user));
    }
}