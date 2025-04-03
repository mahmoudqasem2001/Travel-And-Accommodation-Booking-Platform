using FluentValidation;

namespace AccommodationBookingPlatform.Infrastructure.Presistence.Services.Images;

public class FireBaseConfigValidator : AbstractValidator<FirebaseConfig>
{
    public FireBaseConfigValidator()
    {
        RuleFor(x => x.Bucket)
          .NotEmpty();

        RuleFor(x => x.CredentialsJson)
          .NotEmpty();
    }
}