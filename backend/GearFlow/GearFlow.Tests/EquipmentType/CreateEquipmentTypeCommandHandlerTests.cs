using FluentAssertions;
using GearFlow.Application.Commands.EquipmentType;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.EquipmentType;

public class CreateEquipmentTypeCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.EquipmentType>> _repoMock = new();
    private readonly CreateEquipmentTypeCommandHandle _handler;

    public CreateEquipmentTypeCommandHandlerTests()
    {
        _handler = new CreateEquipmentTypeCommandHandle(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsTypeAndReturnsId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<GearFlow.Domain.Entities.EquipmentType>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CreateEquipmentTypeCommand("Power Tools"), default);

        result.Should().Be(0);
        _repoMock.Verify(r => r.AddAsync(It.Is<GearFlow.Domain.Entities.EquipmentType>(t =>
            t.Name == "Power Tools")), Times.Once);
    }
}
