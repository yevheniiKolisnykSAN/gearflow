using FluentAssertions;
using GearFlow.Application.Commands.Location;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Location;

public class CreateLocationCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.Location>> _repoMock = new();
    private readonly CreateLocationCommandHandle _handler;

    public CreateLocationCommandHandlerTests()
    {
        _handler = new CreateLocationCommandHandle(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsLocationAndReturnsId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<GearFlow.Domain.Entities.Location>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CreateLocationCommand("Warehouse A"), default);

        result.Should().Be(0);
        _repoMock.Verify(r => r.AddAsync(It.Is<GearFlow.Domain.Entities.Location>(l =>
            l.Name == "Warehouse A")), Times.Once);
    }
}
