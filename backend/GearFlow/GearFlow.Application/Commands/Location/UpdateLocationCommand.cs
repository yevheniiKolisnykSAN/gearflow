using MediatR;

namespace GearFlow.Application.Commands.Location;

public record UpdateLocationCommand(int Id, string Name) : IRequest<int>;