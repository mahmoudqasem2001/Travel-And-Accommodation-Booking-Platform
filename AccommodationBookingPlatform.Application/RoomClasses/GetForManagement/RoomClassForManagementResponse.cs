﻿using AccommodationBookingPlatform.Application.Amenities.Common;
using AccommodationBookingPlatform.Application.Discounts.GetById;

namespace AccommodationBookingPlatform.Application.RoomClasses.GetForManagement;

public class RoomClassForManagementResponse
{
    public Guid RoomClassId { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public int AdultsCapacity { get; init; }
    public int ChildrenCapacity { get; init; }
    public decimal PricePerNight { get; init; }
    public IEnumerable<AmenityResponse> Amenities { get; init; }
    public DiscountResponse? ActiveDiscount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
}