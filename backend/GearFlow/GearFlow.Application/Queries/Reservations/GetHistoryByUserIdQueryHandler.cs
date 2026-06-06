using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class GetHistoryByUserIdQueryHandler : IRequestHandler<GetHistoryByUserIdQuery, IEnumerable<Reservation>>
{
    private readonly IReservationRepository _repository;

    public GetHistoryByUserIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Reservation>> Handle(GetHistoryByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHistoryByUserId(request.id);
    }
}