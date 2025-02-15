namespace Domain.Messages;

public static class ReviewMessages
{
    public const string NotFound = "No review found with the given ID.";
    public const string WithIdNotFoundInHotelWithId =
      "No review found with the specified ID for the specified hotel.";
    public const string NotFoundForUserForHotel = "No review found for the specified user and hotel.";
    public const string GuestAlreadyReviewedHotel = "The specified guest has already submitted a review for this hotel.";
}
