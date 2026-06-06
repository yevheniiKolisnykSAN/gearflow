using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class GetReservedDatesQueryHandler : IRequestHandler<GetReservedDatesQuery, IEnumerable<DateTime>>
{
    private readonly IReservationRepository _repository;

    public GetReservedDatesQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DateTime>> Handle(GetReservedDatesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetReservedDates(request.EquipmentId);
    }
}