using MediatR;

namespace GearFlow.Application.Queries.EquipmentType;

public record GetAllEquipmentTypesQuery() : IRequest<IEnumerable<Domain.Entities.EquipmentType>>;