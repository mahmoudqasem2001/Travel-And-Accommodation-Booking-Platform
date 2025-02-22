using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using Application.Users.Register;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Users.Register;

public class RegisterCommandHandler(
  IUserRepository userRepository,
  IMapper mapper,
  IRoleRepository roleRepository,
  IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand>
{
    private readonly IMapper _mapper = mapper;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task Handle(
      RegisterCommand request,
      CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByNameAsync(request.Role, cancellationToken)
                   ?? throw new InvalidRoleException(UserMessages.InvalidRole);

        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new DuplicateEmailUserException(UserMessages.WithEmailExists);
        }

        var userToAdd = _mapper.Map<User>(request);

        userToAdd.Roles.Add(role);

        await _userRepository.CreateAsync(userToAdd, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}