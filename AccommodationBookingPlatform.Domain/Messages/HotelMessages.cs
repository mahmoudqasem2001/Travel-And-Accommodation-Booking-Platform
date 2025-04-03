namespace Domain.Messages;

public static class HotelMessages
{
    public const string NotFound = "No hotel found with the given ID.";
    public const string SameLocationExists = "A hotel already exists at the same location (longitude, latitude).";
    public const string DependentsExist = "The specified hotel still has associated dependents.";
}