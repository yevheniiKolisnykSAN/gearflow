using FluentAssertions;
using GearFlow.Application.Commands.Equipment;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Equipment;

public class UpdateEquipmentCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.Equipment>> _repoMock = new();
    private readonly UpdateEquipmentCommandHandler _handler;

    public UpdateEquipmentCommandHandlerTests()
    {
        _handler = new UpdateEquipmentCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingEquipment_UpdatesAndReturnsId()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 5, Name = "Old" };
        _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(equipment);
        _repoMock.Setup(r => r.UpdateAsync(equipment)).Returns(Task.CompletedTask);

        var command = new UpdateEquipmentCommand(5, "New Drill", 2, "Updated spec", 14, 2, 3);
        var result = await _handler.Handle(command, default);

        result.Should().Be(5);
        equipment.Name.Should().Be("New Drill");
        equipment.LocationId.Should().Be(2);
        equipment.Specification.Should().Be("Updated spec");
        equipment.MaxLoanDays.Should().Be(14);
        equipment.StatusId.Should().Be(2);
        equipment.TypeId.Should().Be(3);
    }

    [Fact]
    public async Task Handle_NotFoundEquipment_ReturnsZero()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.Equipment?)null);

        var result = await _handler.Handle(new UpdateEquipmentCommand(99, "X", 1, "Y", 5, 1, 1), default);

        result.Should().Be(0);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<GearFlow.Domain.Entities.Equipment>()), Times.Never);
    }
}
