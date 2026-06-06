using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetHistoryByUserIdQuery(int id) : IRequest<IEnumerable<Reservation>>;