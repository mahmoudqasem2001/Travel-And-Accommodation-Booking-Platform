namespace Domain.Messages;


public static class CityMessages
{    
    public const string DependentsExist = "The specified city still has associated dependents.";
    public const string NotFound = "No city found with the given ID.";
    public const string PostOfficeExists = "A city with this postal code already exists.";
}