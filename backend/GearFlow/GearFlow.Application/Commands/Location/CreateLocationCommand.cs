using MediatR;

namespace GearFlow.Application.Commands.Location;

public record CreateLocationCommand(string Name) : IRequest<int>;