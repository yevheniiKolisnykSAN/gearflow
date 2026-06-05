using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class GetMyActiveReservationsQueryHandler : IRequestHandler<GetMyActiveReservationsQuery, IEnumerable<Reservation>>
{
    private readonly IReservationRepository _repository;

    public GetMyActiveReservationsQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Reservation>> Handle(GetMyActiveReservationsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActiveByUserIdAsync(request.UserId);
    }
}