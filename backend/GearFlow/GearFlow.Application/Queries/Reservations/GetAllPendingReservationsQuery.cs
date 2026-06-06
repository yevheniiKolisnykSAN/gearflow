using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetAllPendingReservationsQuery() : IRequest<IEnumerable<Reservation>>;