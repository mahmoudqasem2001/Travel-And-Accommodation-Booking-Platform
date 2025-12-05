using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;
using AccommodationBookingPlatform.Tests.Common;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AccommodationBookingPlatform.Tests.Infrastructure.Persistence;

public class RoomRepositoryTests
{
    private readonly InMemoryDbContextFactory _dbContextFactory = new();

    [Fact]
    public async Task ExistsAsync_WhenRoomMatchesPredicate_ReturnsTrue()
    {
        using var context = _dbContextFactory.CreateContext();
        var roomClass = EntityFactory.CreateRoomClass();
        var existingRoom = EntityFactory.CreateRoom(roomClass);
        context.RoomClasses.Add(roomClass);
        context.Rooms.Add(existingRoom);
        await context.SaveChangesAsync();

        var repository = new RoomRepository(context);

        var result = await repository.ExistsAsync(r => r.Number == existingRoom.Number);

        Assert.True(result);
    }

    [Fact]
    public async Task GetForManagementAsync_ProjectsAvailabilityAndPagination()
    {
        using var context = _dbContextFactory.CreateContext();
        var roomClass = EntityFactory.CreateRoomClass();
        var unavailableRoom = EntityFactory.CreateRoom(roomClass, "201");
        var availableRoom = EntityFactory.CreateRoom(roomClass, "202");
        var activeBooking = EntityFactory.CreateBooking(roomClass.Hotel,
            checkIn: DateOnly.FromDateTime(DateTime.UtcNow),
            checkOut: DateOnly.FromDateTime(DateTime.UtcNow));
        activeBooking.Rooms.Add(unavailableRoom);
        unavailableRoom.Bookings.Add(activeBooking);

        context.AddRange(roomClass.Hotel.City, roomClass.Hotel.Owner, roomClass.Hotel, roomClass);
        context.Rooms.AddRange(unavailableRoom, availableRoom);
        context.Bookings.Add(activeBooking);
        await context.SaveChangesAsync();

        var repository = new RoomRepository(context);
        var query = new Query<Room>(r => true, SortOrder.Ascending, "number", pageNumber: 1, pageSize: 10);

        var pagedRooms = await repository.GetForManagementAsync(query);

        Assert.Equal(2, pagedRooms.Items.Count());
        var unavailableProjection = pagedRooms.Items.Single(r => r.Number == "201");
        Assert.False(unavailableProjection.IsAvailable);
        var availableProjection = pagedRooms.Items.Single(r => r.Number == "202");
        Assert.True(availableProjection.IsAvailable);
        Assert.Equal(2, pagedRooms.PaginationMetadata.TotalItemCount);
    }

    [Fact]
    public async Task GetByIdWithRoomClassAsync_ReturnsRoomWithOnlyActiveDiscounts()
    {
        using var context = _dbContextFactory.CreateContext();
        var roomClass = EntityFactory.CreateRoomClass();
        var room = EntityFactory.CreateRoom(roomClass);
        var activeDiscount = new Discount
        {
            Id = Guid.NewGuid(),
            RoomClass = roomClass,
            RoomClassId = roomClass.Id,
            StartDateUtc = DateTime.UtcNow.AddDays(-1),
            EndDateUtc = DateTime.UtcNow.AddDays(1),
            Percentage = 10,
            CreatedAtUtc = DateTime.UtcNow
        };
        var expiredDiscount = new Discount
        {
            Id = Guid.NewGuid(),
            RoomClass = roomClass,
            RoomClassId = roomClass.Id,
            StartDateUtc = DateTime.UtcNow.AddDays(-10),
            EndDateUtc = DateTime.UtcNow.AddDays(-5),
            Percentage = 5,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
        };
        roomClass.Discounts.Add(activeDiscount);
        roomClass.Discounts.Add(expiredDiscount);

        context.AddRange(roomClass.Hotel.City, roomClass.Hotel.Owner, roomClass.Hotel, roomClass, room);
        context.Discounts.AddRange(activeDiscount, expiredDiscount);
        await context.SaveChangesAsync();

        var repository = new RoomRepository(context);

        var result = await repository.GetByIdWithRoomClassAsync(room.Id);

        Assert.NotNull(result);
        Assert.NotNull(result!.RoomClass);
        Assert.Single(result.RoomClass.Discounts);
        Assert.Equal(activeDiscount.Id, result.RoomClass.Discounts.Single().Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoomDoesNotExist_ThrowsNotFoundException()
    {
        using var context = _dbContextFactory.CreateContext();
        IRoomRepository repository = new RoomRepository(context);
        var detachedRoom = EntityFactory.CreateRoom();

        var action = () => repository.UpdateAsync(detachedRoom);

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_RemovesRoomWhenExists()
    {
        using var context = _dbContextFactory.CreateContext();
        var roomClass = EntityFactory.CreateRoomClass();
        var room = EntityFactory.CreateRoom(roomClass);
        context.AddRange(roomClass.Hotel.City, roomClass.Hotel.Owner, roomClass.Hotel, roomClass, room);
        await context.SaveChangesAsync();

        IRoomRepository repository = new RoomRepository(context);

        await repository.DeleteAsync(room.Id);
        await context.SaveChangesAsync();

        var remainingRooms = await context.Rooms.ToListAsync();
        Assert.Empty(remainingRooms);
    }
}
