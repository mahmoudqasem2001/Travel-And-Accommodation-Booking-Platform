using AccommodationBookingPlatform.Application.Bookings.GetInvoiceAsPdf;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Exceptions;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Moq;
using AccommodationBookingPlatform.Tests.Common;

namespace AccommodationBookingPlatform.Tests.Application.Bookings;

public class GetInvoiceAsPdfQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IPdfService> _pdfServiceMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();

    [Fact]
    public async Task Handle_UserDoesNotExist_ThrowsNotFoundException()
    {
        var guestId = Guid.NewGuid();
        _userContextMock.SetupGet(c => c.Id).Returns(guestId);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(guestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        var action = () => handler.Handle(CreateQuery(), CancellationToken.None);

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal(UserMessages.NotFound, exception.Message);
        _bookingRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), true, It.IsAny<CancellationToken>()), Times.Never);
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

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(CreateQuery(), CancellationToken.None));
        _bookingRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), true, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ThrowsNotFoundException()
    {
        SetupGuestContext();
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(CreateQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPdfBytes()
    {
        var booking = CreateBooking();
        var expectedPdf = new byte[] { 1, 2, 3 };
        SetupGuestContext(booking.GuestId);
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.GuestId, booking.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _pdfServiceMock.Setup(p => p.GeneratePdfFromHtmlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPdf);

        var handler = CreateHandler();
        var query = CreateQuery(booking.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(expectedPdf, result);
        _pdfServiceMock.Verify(p => p.GeneratePdfFromHtmlAsync(
            It.Is<string>(html =>
                html.Contains(booking.Hotel.Name) &&
                html.Contains(booking.Invoice.First().RoomNumber) &&
                html.Contains(booking.TotalPrice.ToString("F2"))),
            It.IsAny<CancellationToken>()), Times.Once);
        _bookingRepositoryMock.Verify(r => r.GetByIdAsync(booking.GuestId, booking.Id, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    private GetInvoiceAsPdfQueryHandler CreateHandler()
    {
        return new GetInvoiceAsPdfQueryHandler(
            _bookingRepositoryMock.Object,
            _pdfServiceMock.Object,
            _userRepositoryMock.Object,
            _userContextMock.Object);
    }

    private void SetupGuestContext(Guid? guestId = null)
    {
        var id = guestId ?? Guid.NewGuid();
        _userContextMock.SetupGet(c => c.Id).Returns(id);
        _userContextMock.SetupGet(c => c.Role).Returns(UserRoles.Guest);
        _userRepositoryMock.Setup(r => r.ExistsByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static GetInvoiceAsPdfQuery CreateQuery(Guid? bookingId = null)
    {
        return new GetInvoiceAsPdfQuery
        {
            BookingId = bookingId ?? Guid.NewGuid()
        };
    }

    private static Booking CreateBooking()
    {
        var hotel = EntityFactory.CreateHotel();
        var roomClass = EntityFactory.CreateRoomClass(hotel);
        var room = EntityFactory.CreateRoom(roomClass);
        var booking = EntityFactory.CreateBooking(hotel, checkIn: new DateOnly(2025, 1, 1), checkOut: new DateOnly(2025, 1, 3));
        booking.Id = Guid.NewGuid();
        booking.Invoice.Add(new InvoiceRecord
        {
            Id = Guid.NewGuid(),
            Booking = booking,
            RoomNumber = room.Number,
            RoomClassName = room.RoomClass.Name,
            PriceAtBooking = room.RoomClass.PricePerNight,
            DiscountPercentageAtBooking = 5
        });
        booking.TotalPrice = 500;
        return booking;
    }
}
