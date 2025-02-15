namespace Domain.Exceptions;

public class ResourceHasDependentsException(string message) : ConflictException(message)
{
    public override string Title => "This resource has existing dependents";
}
