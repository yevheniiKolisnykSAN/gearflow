using FluentAssertions;
using GearFlow.Application.Commands.Reservation;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GearFlow.Tests.Reservation;

public class CompleteReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IEquipmentRepository> _equipmentRepoMock = new();
    private readonly Mock<IRepository<Defect>> _defectRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly CompleteReservationCommandHandler _handler;

    public CompleteReservationCommandHandlerTests()
    {
        _configurationMock.Setup(c => c["Email:AdminEmail"]).Returns("admin@test.com");

        _handler = new CompleteReservationCommandHandler(
            _reservationRepoMock.Object,
            _userRepoMock.Object,
            _equipmentRepoMock.Object,
            _defectRepoMock.Object,
            _emailServiceMock.Object,
            _configurationMock.Object);
    }

    private Domain.Entities.Reservation BuildReservation(int id = 1, int userId = 10, int equipmentId = 5) =>
        new() { Id = id, UserId = userId, EquipmentId = equipmentId, Status = ReservationStatus.Active };

    [Fact]
    public async Task Handle_ValidRequest_SetsPendingAndReturnsTrue()
    {
        var reservation = BuildReservation();
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 5 };

        _reservationRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(equipment);
        _reservationRepoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CompleteReservationCommand(1, 10, 2), default);

        result.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.PendingAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithDefectComment_AddsDefect()
    {
        var reservation = BuildReservation();
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 5 };

        _reservationRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(equipment);
        _defectRepoMock.Setup(r => r.AddAsync(It.IsAny<Defect>())).Returns(Task.CompletedTask);
        _reservationRepoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);

        await _handler.Handle(new CompleteReservationCommand(1, 10, 2, "Cracked screen"), default);

        _defectRepoMock.Verify(r => r.AddAsync(It.Is<Defect>(d =>
            d.Comment == "Cracked screen" &&
            d.EquipmentId == 5 &&
            d.Status == DefectStatus.New)), Times.Once);
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ThrowsKeyNotFoundException()
    {
        _reservationRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Domain.Entities.Reservation?)null);

        var act = () => _handler.Handle(new CompleteReservationCommand(99, 1, 2), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Reservation not found");
    }

    [Fact]
    public async Task Handle_WrongUser_ThrowsInvalidOperationException()
    {
        var reservation = BuildReservation(userId: 10);
        _reservationRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);

        var act = () => _handler.Handle(new CompleteReservationCommand(1, 99, 2), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only user can complete reservation");
    }

    [Fact]
    public async Task Handle_EquipmentNotFound_ThrowsKeyNotFoundException()
    {
        var reservation = BuildReservation(userId: 10, equipmentId: 5);
        _reservationRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((GearFlow.Domain.Entities.Equipment?)null);

        var act = () => _handler.Handle(new CompleteReservationCommand(1, 10, 2), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Equipment not found");
    }
}
