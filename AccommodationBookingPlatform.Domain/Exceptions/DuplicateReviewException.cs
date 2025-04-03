namespace Domain.Exceptions;

public class DuplicateReviewException(string message) : ConflictException(message)
{
    public override string Title => "This hotel has already been reviewed";
}
