using MediatR;
namespace GearFlow.Application.Queries.Location;

public record GetAllLocationsQuery() : IRequest<IEnumerable<Domain.Entities.Location>>;