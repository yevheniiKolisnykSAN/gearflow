using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetActiveReservationsListByEquipmentIdQuery(int id) : IRequest<IEnumerable<Reservation>>;