using AccommodationBookingPlatform.Application.Bookings.Create;
using AccommodationBookingPlatform.Application.Mapping;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Exceptions;
using AutoMapper;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Moq;
using AccommodationBookingPlatform.Tests.Common;

namespace AccommodationBookingPlatform.Tests.Application.Bookings;

public class CreateBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IHotelRepository> _hotelRepositoryMock = new();
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IPdfService> _pdfServiceMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly IMapper _mapper;

    public CreateBookingCommandHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<BookingProfile>())
            .CreateMapper();

        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<EmailRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _pdfServiceMock.Setup(p => p.GeneratePdfFromHtmlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<byte>());
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ThrowsNotFoundException()
    {
        var guestId = Guid.NewGuid();
        _userContextMock.SetupGet(c => c.Id).Returns(guestId);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(guestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(CreateCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UserIsNotGuest_ThrowsForbiddenException()
    {
        var guestId = Guid.NewGuid();
        _userContextMock.SetupGet(c => c.Id).Returns(guestId);
        _userContextMock.SetupGet(c => c.Role).Returns(UserRoles.Admin);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(guestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(CreateCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_HotelDoesNotExist_ThrowsNotFoundException()
    {
        var guestId = Guid.NewGuid();
        _userContextMock.SetupGet(c => c.Id).Returns(guestId);
        _userContextMock.SetupGet(c => c.Role).Returns(UserRoles.Guest);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(guestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(CreateCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RoomIsMissing_RollsBackAndThrowsNotFoundException()
    {
        var hotel = EntityFactory.CreateHotel();
        var command = CreateCommand(hotelId: hotel.Id);
        SetupGuestContext();
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotel.Id, false, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        _roomRepositoryMock.Setup(r => r.GetByIdWithRoomClassAsync(command.RoomIds.First(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RoomFromDifferentHotel_RollsBackAndThrowsRoomsNotInHotelException()
    {
        var hotel = EntityFactory.CreateHotel();
        var differentHotel = EntityFactory.CreateHotel();
        var roomClass = EntityFactory.CreateRoomClass(differentHotel);
        var room = EntityFactory.CreateRoom(roomClass);
        var command = CreateCommand(hotelId: hotel.Id, roomIds: new[] { room.Id });
        SetupGuestContext();
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotel.Id, false, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        _roomRepositoryMock.Setup(r => r.GetByIdWithRoomClassAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<RoomsNotInHotelException>(() => handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RoomUnavailable_RollsBackAndThrowsRoomNotAvailableException()
    {
        var (hotel, room) = CreateHotelWithRoom();
        var command = CreateCommand(hotelId: hotel.Id, roomIds: new[] { room.Id });
        SetupGuestContext();
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotel.Id, false, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        _roomRepositoryMock.Setup(r => r.GetByIdWithRoomClassAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _roomRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Room, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<RoomNotAvailableException>(() => handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesBookingAndSendsInvoice()
    {
        var (hotel, firstRoom) = CreateHotelWithRoom();
        var secondRoom = EntityFactory.CreateRoom(firstRoom.RoomClass, "102");
        var guestEmail = "guest@example.com";
        var guestId = Guid.NewGuid();
        var checkIn = new DateOnly(2025, 1, 1);
        var checkOut = checkIn.AddDays(2);
        var discount = new Discount
        {
            Id = Guid.NewGuid(),
            RoomClassId = firstRoom.RoomClassId,
            RoomClass = firstRoom.RoomClass,
            Percentage = 10,
            StartDateUtc = DateTime.UtcNow.AddDays(-1),
            EndDateUtc = DateTime.UtcNow.AddDays(1),
            CreatedAtUtc = DateTime.UtcNow
        };
        firstRoom.RoomClass.Discounts.Add(discount);
        var createdBookingId = Guid.NewGuid();
        var createdBooking = (Booking?)null;

        _userContextMock.SetupGet(c => c.Id).Returns(guestId);
        _userContextMock.SetupGet(c => c.Role).Returns(UserRoles.Guest);
        _userContextMock.SetupGet(c => c.Email).Returns(guestEmail);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(guestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotel.Id, false, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        _roomRepositoryMock.SetupSequence(r => r.GetByIdWithRoomClassAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(firstRoom)
            .ReturnsAsync(secondRoom);
        _roomRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Room, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookingRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking booking, CancellationToken _) =>
            {
                booking.Id = createdBookingId;
                createdBooking = booking;
                return booking;
            });
        _dateTimeProviderMock.Setup(d => d.GetCurrentDateUTC()).Returns(checkIn);

        var handler = CreateHandler();
        var command = CreateCommand(hotel.Id, new[] { firstRoom.Id, secondRoom.Id }, checkIn, checkOut);

        var response = await handler.Handle(command, CancellationToken.None);

        var expectedTotalPricePerNight = 2 * (firstRoom.RoomClass.PricePerNight * 0.9m);
        Assert.Equal(createdBookingId, response.Id);
        Assert.Equal(hotel.Name, response.HotelName);
        Assert.Equal(expectedTotalPricePerNight * (checkOut.DayNumber - checkIn.DayNumber), response.TotalPrice);
        Assert.Equal(checkIn, response.BookingDateUtc);
        Assert.NotNull(createdBooking);
        Assert.Equal(2, createdBooking!.Invoice.Count);
        Assert.All(createdBooking.Invoice, invoice =>
        {
            Assert.Equal(discount.Percentage, invoice.DiscountPercentageAtBooking);
            Assert.Equal(firstRoom.RoomClass.Name, invoice.RoomClassName);
        });
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _pdfServiceMock.Verify(p => p.GeneratePdfFromHtmlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _emailServiceMock.Verify(e => e.SendAsync(
            It.Is<EmailRequest>(request =>
                request.ToEmails.Single() == guestEmail &&
                request.Attachments.Single().Name == "invoice.pdf"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateBookingCommandHandler CreateHandler()
    {
        return new CreateBookingCommandHandler(
            _hotelRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _bookingRepositoryMock.Object,
            _emailServiceMock.Object,
            _userContextMock.Object,
            _pdfServiceMock.Object,
            _dateTimeProviderMock.Object,
            _userRepositoryMock.Object);
    }

    private void SetupGuestContext()
    {
        var guestId = Guid.NewGuid();
        _userContextMock.SetupGet(c => c.Id).Returns(guestId);
        _userContextMock.SetupGet(c => c.Role).Returns(UserRoles.Guest);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(guestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static CreateBookingCommand CreateCommand(
        Guid? hotelId = null,
        IEnumerable<Guid>? roomIds = null,
        DateOnly? checkIn = null,
        DateOnly? checkOut = null)
    {
        var defaultCheckIn = checkIn ?? DateOnly.FromDateTime(DateTime.UtcNow);
        return new CreateBookingCommand
        {
            HotelId = hotelId ?? Guid.NewGuid(),
            RoomIds = roomIds ?? new[] { Guid.NewGuid() },
            CheckInDateUtc = defaultCheckIn,
            CheckOutDateUtc = checkOut ?? defaultCheckIn.AddDays(1),
            GuestRemarks = "Looking forward to it",
            PaymentMethod = PaymentMethod.Visa
        };
    }

    private static (Hotel Hotel, Room Room) CreateHotelWithRoom()
    {
        var hotel = EntityFactory.CreateHotel();
        var roomClass = EntityFactory.CreateRoomClass(hotel);
        var room = EntityFactory.CreateRoom(roomClass);
        return (hotel, room);
    }
}
