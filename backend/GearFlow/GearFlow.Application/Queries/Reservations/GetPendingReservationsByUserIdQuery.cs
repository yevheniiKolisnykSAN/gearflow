using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetPendingReservationsByUserIdQuery(int id) : IRequest<IEnumerable<Reservation>>;