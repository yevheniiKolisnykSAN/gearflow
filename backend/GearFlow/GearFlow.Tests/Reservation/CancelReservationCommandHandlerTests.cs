using FluentAssertions;
using GearFlow.Application.Commands.Reservation;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Reservation;

public class CancelReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _repoMock = new();
    private readonly CancelReservationCommandHandler _handler;

    public CancelReservationCommandHandlerTests()
    {
        _handler = new CancelReservationCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ActiveFutureReservationByOwner_CancelsAndReturnsTrue()
    {
        var reservation = new Domain.Entities.Reservation
        {
            Id = 1,
            UserId = 5,
            Status = ReservationStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(3)
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _repoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CancelReservationCommand(1, 5, 2), default);

        result.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_AdminCancelsAnyReservation_Succeeds()
    {
        var reservation = new Domain.Entities.Reservation
        {
            Id = 1,
            UserId = 5,
            Status = ReservationStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(1)
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _repoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CancelReservationCommand(1, 99, 1), default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Domain.Entities.Reservation?)null);

        var act = () => _handler.Handle(new CancelReservationCommand(99, 1, 2), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Reservation not found");
    }

    [Fact]
    public async Task Handle_NonActiveReservation_ThrowsInvalidOperationException()
    {
        var reservation = new Domain.Entities.Reservation
        {
            Id = 1,
            UserId = 5,
            Status = ReservationStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(1)
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);

        var act = () => _handler.Handle(new CancelReservationCommand(1, 5, 2), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only active reservations can be canceled");
    }

    [Fact]
    public async Task Handle_DifferentUserWithRoleUser_ThrowsInvalidOperationException()
    {
        var reservation = new Domain.Entities.Reservation
        {
            Id = 1,
            UserId = 5,
            Status = ReservationStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(3)
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);

        var act = () => _handler.Handle(new CancelReservationCommand(1, 99, 2), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only user can complete reservation");
    }

    [Fact]
    public async Task Handle_AlreadyStartedReservation_ThrowsInvalidOperationException()
    {
        var reservation = new Domain.Entities.Reservation
        {
            Id = 1,
            UserId = 5,
            Status = ReservationStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-1)
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);

        var act = () => _handler.Handle(new CancelReservationCommand(1, 5, 2), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot cancel reservation that has already started");
    }
}
