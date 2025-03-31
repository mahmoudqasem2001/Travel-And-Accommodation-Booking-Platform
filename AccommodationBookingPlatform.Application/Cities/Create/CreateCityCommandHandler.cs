using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Cities.Create;

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, CityResponse>
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCityCommandHandler(
      ICityRepository cityRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper)
    {
        _cityRepository = cityRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CityResponse> Handle(
      CreateCityCommand request,
      CancellationToken cancellationToken = default)
    {
        if (await _cityRepository.ExistsAsync(c => c.PostOffice == request.PostOffice, cancellationToken))
        {
            throw new DuplicateCityPostOfficeException(CityMessages.PostOfficeExists);
        }

        var createdCity = await _cityRepository.CreateAsync(
          _mapper.Map<City>(request),
          cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CityResponse>(createdCity);
    }
}