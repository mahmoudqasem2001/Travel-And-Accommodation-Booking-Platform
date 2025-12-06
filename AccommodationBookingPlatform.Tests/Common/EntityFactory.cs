using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;

namespace AccommodationBookingPlatform.Tests.Common;

public static class EntityFactory
{
    public static City CreateCity(Guid? id = null)
    {
        return new City
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Test City",
            Country = "Test Country",
            PostOffice = "00000",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static Owner CreateOwner(Guid? id = null)
    {
        return new Owner
        {
            Id = id ?? Guid.NewGuid(),
            FirstName = "Owner",
            LastName = "Example",
            Email = "owner@example.com",
            PhoneNumber = "1234567890"
        };
    }

    public static Hotel CreateHotel(City? city = null, Owner? owner = null)
    {
        city ??= CreateCity();
        owner ??= CreateOwner();

        return new Hotel
        {
            Id = Guid.NewGuid(),
            City = city,
            CityId = city.Id,
            Owner = owner,
            OwnerId = owner.Id,
            Name = "Test Hotel",
            ReviewsRating = 4.5,
            StarRating = 4,
            Longitude = 0,
            Latitude = 0,
            PhoneNumber = "1234567890",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static RoomClass CreateRoomClass(Hotel? hotel = null)
    {
        hotel ??= CreateHotel();

        return new RoomClass
        {
            Id = Guid.NewGuid(),
            Hotel = hotel,
            HotelId = hotel.Id,
            Name = "Standard",
            AdultsCapacity = 2,
            ChildrenCapacity = 1,
            PricePerNight = 100,
            RoomType = RoomType.Budget,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static Room CreateRoom(RoomClass? roomClass = null, string number = "101")
    {
        roomClass ??= CreateRoomClass();

        return new Room
        {
            Id = Guid.NewGuid(),
            RoomClassId = roomClass.Id,
            RoomClass = roomClass,
            Number = number,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static User CreateUser(Guid? id = null, string? email = null)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = email ?? "test.user@example.com",
            Password = "password"
        };
    }

    public static Booking CreateBooking(Hotel hotel, User? guest = null, DateOnly? checkIn = null, DateOnly? checkOut = null)
    {
        guest ??= CreateUser();
        var checkInDate = checkIn ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var checkOutDate = checkOut ?? checkInDate.AddDays(1);

        return new Booking
        {
            Id = Guid.NewGuid(),
            Guest = guest,
            GuestId = guest.Id,
            Hotel = hotel,
            HotelId = hotel.Id,
            Rooms = new List<Room>(),
            BookingDateUtc = checkInDate,
            CheckInDateUtc = checkInDate,
            CheckOutDateUtc = checkOutDate,
            PaymentMethod = PaymentMethod.Visa,
            TotalPrice = 200
        };
    }
}
