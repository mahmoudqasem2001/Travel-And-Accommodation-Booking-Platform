namespace Domain.Messages;

public static class DiscountMessages
{
    public const string NotFound = "No discount found with the given ID.";
    public const string NotFoundInRoomClass = "No discount found with the given ID in the specified room class.";
    public const string InDateIntervalExists = "Another discount is already active within this date range.";
}
