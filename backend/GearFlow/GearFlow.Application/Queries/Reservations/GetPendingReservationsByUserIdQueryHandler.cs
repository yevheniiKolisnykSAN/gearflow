using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class
    GetPendingReservationsByUserIdQueryHandler : IRequestHandler<GetPendingReservationsByUserIdQuery,
    IEnumerable<Reservation>>
{
    private readonly IReservationRepository _repository;

    public GetPendingReservationsByUserIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Reservation>> Handle(GetPendingReservationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllPendingReservationsByUserId(request.id);
    }
}