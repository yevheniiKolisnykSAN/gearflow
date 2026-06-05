using MediatR;

namespace GearFlow.Application.Queries.EquipmentStatus;

public record GetAllEquipmentStatusesQuery : IRequest<IEnumerable<Domain.Entities.EquipmentStatus>>;