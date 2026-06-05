using MediatR;

namespace GearFlow.Application.Commands.Location;

public record ArchiveLocationCommand(int Id) : IRequest<bool>;