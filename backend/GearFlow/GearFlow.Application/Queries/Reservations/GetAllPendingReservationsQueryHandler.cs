using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class GetAllPendingReservationsQueryHandler : IRequestHandler<GetAllPendingReservationsQuery, IEnumerable<Reservation>>
{
    private readonly IReservationRepository _repository;

    public GetAllPendingReservationsQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Reservation>> Handle(GetAllPendingReservationsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllPendingReservations();
    }
}