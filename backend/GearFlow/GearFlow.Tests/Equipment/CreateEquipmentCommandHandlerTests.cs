using FluentAssertions;
using GearFlow.Application.Commands.Equipment;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Equipment;

public class CreateEquipmentCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.Equipment>> _repoMock = new();
    private readonly CreateEquipmentCommandHandler _handler;

    public CreateEquipmentCommandHandlerTests()
    {
        _handler = new CreateEquipmentCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsEquipmentAndReturnsId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<GearFlow.Domain.Entities.Equipment>()))
            .Returns(Task.CompletedTask);

        var command = new CreateEquipmentCommand("SN-001", "Drill", "Power drill", 7, 1, 1, 1);
        var result = await _handler.Handle(command, default);

        result.Should().Be(0);
        _repoMock.Verify(r => r.AddAsync(It.Is<GearFlow.Domain.Entities.Equipment>(e =>
            e.SerialNumber == "SN-001" &&
            e.Name == "Drill" &&
            e.Specification == "Power drill" &&
            e.MaxLoanDays == 7 &&
            e.StatusId == 1 &&
            e.TypeId == 1 &&
            e.LocationId == 1)), Times.Once);
    }
}
