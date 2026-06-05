using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetReservationHistoryByEquipmentIdQuery(int EquipmentId) : IRequest<IEnumerable<Reservation>>;