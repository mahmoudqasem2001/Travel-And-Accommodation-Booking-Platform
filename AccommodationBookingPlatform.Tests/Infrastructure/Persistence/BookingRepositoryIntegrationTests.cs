using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;
using AccommodationBookingPlatform.Tests.Common;

namespace AccommodationBookingPlatform.Tests.Infrastructure.Persistence;

public class BookingRepositoryIntegrationTests
{
    private readonly InMemoryDbContextFactory _dbContextFactory = new();

    [Fact]
    public async Task GetByIdAsync_WithInvoiceIncluded_ReturnsBookingWithInvoiceRecords()
    {
        using var context = _dbContextFactory.CreateContext();
        var booking = CreateBookingWithInvoice();
        context.AddRange(booking.Hotel.City, booking.Hotel.Owner, booking.Hotel);
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context);

        var result = await repository.GetByIdAsync(booking.Id, booking.GuestId, includeInvoice: true);

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Invoice);
        var invoiceRecord = result.Invoice.Single();
        Assert.Equal(booking.Invoice.Single().RoomNumber, invoiceRecord.RoomNumber);
        Assert.Equal(booking.Invoice.Single().PriceAtBooking, invoiceRecord.PriceAtBooking);
    }

    [Fact]
    public async Task GetRecentBookingsInDifferentHotelsByGuestId_ReturnsDistinctHotelsOrderedByCheckIn()
    {
        using var context = _dbContextFactory.CreateContext();
        var (latestBooking, olderBooking) = CreateBookingsInDifferentHotels();
        context.AddRange(
            latestBooking.Hotel.City, latestBooking.Hotel.Owner, latestBooking.Hotel,
            olderBooking.Hotel.City, olderBooking.Hotel.Owner, olderBooking.Hotel);
        context.Bookings.AddRange(latestBooking, olderBooking);
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context);

        var result = await repository.GetRecentBookingsInDifferentHotelsByGuestId(latestBooking.GuestId, count: 5);

        var bookings = result.ToList();
        Assert.Equal(2, bookings.Count);
        Assert.Equal(latestBooking.Id, bookings[0].Id);
        Assert.Equal(olderBooking.Id, bookings[1].Id);
        Assert.NotEqual(bookings[0].HotelId, bookings[1].HotelId);
    }

    private static Booking CreateBookingWithInvoice()
    {
        var hotel = EntityFactory.CreateHotel();
        var roomClass = EntityFactory.CreateRoomClass(hotel);
        var room = EntityFactory.CreateRoom(roomClass);
        var booking = EntityFactory.CreateBooking(hotel);
        booking.Id = Guid.NewGuid();
        booking.Invoice.Add(new InvoiceRecord
        {
            Id = Guid.NewGuid(),
            Booking = booking,
            BookingId = booking.Id,
            RoomId = room.Id,
            RoomClassName = roomClass.Name,
            RoomNumber = room.Number,
            PriceAtBooking = roomClass.PricePerNight,
            DiscountPercentageAtBooking = 10
        });
        return booking;
    }

    private static (Booking Latest, Booking Older) CreateBookingsInDifferentHotels()
    {
        var guest = EntityFactory.CreateUser();

        var olderHotel = EntityFactory.CreateHotel();
        var olderBooking = EntityFactory.CreateBooking(
            olderHotel,
            guest,
            checkIn: new DateOnly(2024, 1, 1),
            checkOut: new DateOnly(2024, 1, 3));
        olderBooking.Id = Guid.NewGuid();
        olderBooking.GuestId = guest.Id;
        olderBooking.Guest = guest;

        var latestHotel = EntityFactory.CreateHotel();
        var latestBooking = EntityFactory.CreateBooking(
            latestHotel,
            guest,
            checkIn: new DateOnly(2025, 1, 1),
            checkOut: new DateOnly(2025, 1, 3));
        latestBooking.Id = Guid.NewGuid();
        latestBooking.GuestId = guest.Id;
        latestBooking.Guest = guest;

        return (latestBooking, olderBooking);
    }
}
