using FluentAssertions;
using GearFlow.Application.Commands.Reservation;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Reservation;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IEquipmentRepository> _equipmentRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly CreateReservationCommandHandler _handler;

    public CreateReservationCommandHandlerTests()
    {
        _handler = new CreateReservationCommandHandler(
            _reservationRepoMock.Object,
            _equipmentRepoMock.Object,
            _userRepoMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidFutureReservation_CreatesAndReturnsId()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, MaxLoanDays = 14, StatusId = 1 };
        var start = DateTime.UtcNow.AddDays(2);
        var end = DateTime.UtcNow.AddDays(7);

        _equipmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);
        _reservationRepoMock.Setup(r => r.GetConflictingReservationAsync(1, start, end))
            .ReturnsAsync((Domain.Entities.Reservation?)null);
        _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Reservation>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CreateReservationCommand(1, start, end, 10), default);

        result.Should().Be(0);
        _reservationRepoMock.Verify(r => r.AddAsync(It.Is<Domain.Entities.Reservation>(res =>
            res.EquipmentId == 1 &&
            res.UserId == 10 &&
            res.Status == ReservationStatus.Active)), Times.Once);
    }

    [Fact]
    public async Task Handle_EquipmentNotFound_ThrowsKeyNotFoundException()
    {
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.Equipment?)null);

        var act = () => _handler.Handle(
            new CreateReservationCommand(99, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 1), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Equipment not found");
    }

    [Fact]
    public async Task Handle_ExceedsMaxLoanDays_ThrowsInvalidOperationException()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, MaxLoanDays = 3, StatusId = 1 };
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(10);

        _equipmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);

        var act = () => _handler.Handle(new CreateReservationCommand(1, start, end, 1), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Max loan days is 3");
    }

    [Fact]
    public async Task Handle_ConflictingReservation_ThrowsInvalidOperationException()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, MaxLoanDays = 30, StatusId = 1 };
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(5);
        var existing = new Domain.Entities.Reservation { Id = 42 };

        _equipmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);
        _reservationRepoMock.Setup(r => r.GetConflictingReservationAsync(1, start, end)).ReturnsAsync(existing);

        var act = () => _handler.Handle(new CreateReservationCommand(1, start, end, 1), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Equipment already reserved for this period");
    }

    [Fact]
    public async Task Handle_StartDateTodayOrPast_SetsEquipmentStatusToInUse()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, MaxLoanDays = 30, StatusId = 1 };
        var start = DateTime.UtcNow.Date;
        var end = DateTime.UtcNow.AddDays(5);

        _equipmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);
        _reservationRepoMock.Setup(r => r.GetConflictingReservationAsync(1, start, end))
            .ReturnsAsync((Domain.Entities.Reservation?)null);
        _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Reservation>()))
            .Returns(Task.CompletedTask);
        _equipmentRepoMock.Setup(r => r.UpdateAsync(equipment)).Returns(Task.CompletedTask);

        await _handler.Handle(new CreateReservationCommand(1, start, end, 1), default);

        equipment.StatusId.Should().Be(2);
        _equipmentRepoMock.Verify(r => r.UpdateAsync(equipment), Times.Once);
    }
}
