using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetReservedDatesQuery(int EquipmentId) : IRequest<IEnumerable<DateTime>>;