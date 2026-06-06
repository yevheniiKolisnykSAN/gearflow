using FluentAssertions;
using GearFlow.Application.Commands.EquipmentType;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.EquipmentType;

public class UpdateEquipmentTypeCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.EquipmentType>> _repoMock = new();
    private readonly UpdateEquipmentTypeCommandHandle _handler;

    public UpdateEquipmentTypeCommandHandlerTests()
    {
        _handler = new UpdateEquipmentTypeCommandHandle(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingType_UpdatesAndReturnsId()
    {
        var type = new GearFlow.Domain.Entities.EquipmentType { Id = 2, Name = "Old" };
        _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(type);
        _repoMock.Setup(r => r.UpdateAsync(type)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateEquipmentTypeCommand(2, "Power Tools"), default);

        result.Should().Be(2);
        type.Name.Should().Be("Power Tools");
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsZero()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.EquipmentType?)null);

        var result = await _handler.Handle(new UpdateEquipmentTypeCommand(99, "X"), default);

        result.Should().Be(0);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<GearFlow.Domain.Entities.EquipmentType>()), Times.Never);
    }
}
