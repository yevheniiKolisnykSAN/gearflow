using FluentAssertions;
using GearFlow.Application.Commands.Reservation;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Reservation;

public class ConfirmReturnCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IRepository<Defect>> _defectRepoMock = new();
    private readonly Mock<IEquipmentRepository> _equipmentRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly ConfirmReturnCommandHandler _handler;

    public ConfirmReturnCommandHandlerTests()
    {
        _handler = new ConfirmReturnCommandHandler(
            _reservationRepoMock.Object,
            _defectRepoMock.Object,
            _equipmentRepoMock.Object,
            _emailServiceMock.Object,
            _userRepoMock.Object);
    }

    private Domain.Entities.Reservation BuildPendingReservation(int equipmentId = 5, int userId = 10) =>
        new()
        {
            Id = 1,
            UserId = userId,
            EquipmentId = equipmentId,
            Status = ReservationStatus.Pending
        };

    [Fact]
    public async Task Handle_ValidPendingReservation_CompletesAndSetsEquipmentAvailable()
    {
        var reservation = BuildPendingReservation();
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 5, StatusId = 2 };

        _reservationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(equipment);
        _reservationRepoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);
        _equipmentRepoMock.Setup(r => r.UpdateAsync(equipment)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ConfirmReturnCommand(1), default);

        result.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Completed);
        reservation.CompletedAt.Should().NotBeNull();
        equipment.StatusId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithDefectComment_NoExistingDefect_CreatesNewDefect()
    {
        var reservation = BuildPendingReservation();
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 5 };

        _reservationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(equipment);
        _defectRepoMock.Setup(r => r.AddAsync(It.IsAny<Defect>())).Returns(Task.CompletedTask);
        _reservationRepoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);
        _equipmentRepoMock.Setup(r => r.UpdateAsync(equipment)).Returns(Task.CompletedTask);

        await _handler.Handle(new ConfirmReturnCommand(1, "New scratch"), default);

        _defectRepoMock.Verify(r => r.AddAsync(It.Is<Defect>(d =>
            d.Comment == "New scratch" &&
            d.EquipmentId == 5 &&
            d.Status == DefectStatus.New)), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDefectComment_ExistingDefect_UpdatesDefect()
    {
        var existingDefect = new Defect { Id = 10, Comment = "Old comment" };
        var reservation = BuildPendingReservation();
        reservation.Defect = existingDefect;
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 5 };

        _reservationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(equipment);
        _defectRepoMock.Setup(r => r.UpdateAsync(existingDefect)).Returns(Task.CompletedTask);
        _reservationRepoMock.Setup(r => r.UpdateAsync(reservation)).Returns(Task.CompletedTask);
        _equipmentRepoMock.Setup(r => r.UpdateAsync(equipment)).Returns(Task.CompletedTask);

        await _handler.Handle(new ConfirmReturnCommand(1, "Updated comment"), default);

        existingDefect.Comment.Should().Be("Updated comment");
        _defectRepoMock.Verify(r => r.UpdateAsync(existingDefect), Times.Once);
        _defectRepoMock.Verify(r => r.AddAsync(It.IsAny<Defect>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ThrowsKeyNotFoundException()
    {
        _reservationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((Domain.Entities.Reservation?)null);

        var act = () => _handler.Handle(new ConfirmReturnCommand(99), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Reservation not found");
    }

    [Fact]
    public async Task Handle_NonPendingReservation_ThrowsInvalidOperationException()
    {
        var reservation = BuildPendingReservation();
        reservation.Status = ReservationStatus.Active;
        _reservationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(reservation);

        var act = () => _handler.Handle(new ConfirmReturnCommand(1), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Reservation should have \"Pending\" status");
    }

    [Fact]
    public async Task Handle_EquipmentNotFound_ThrowsKeyNotFoundException()
    {
        var reservation = BuildPendingReservation(equipmentId: 5);
        _reservationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(reservation);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((GearFlow.Domain.Entities.Equipment?)null);

        var act = () => _handler.Handle(new ConfirmReturnCommand(1), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Equipment not found");
    }
}
