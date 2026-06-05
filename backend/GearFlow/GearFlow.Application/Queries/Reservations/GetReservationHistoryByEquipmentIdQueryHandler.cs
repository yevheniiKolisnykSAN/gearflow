using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class
    GetReservationHistoryByEquipmentIdQueryHandler :
    IRequestHandler<GetReservationHistoryByEquipmentIdQuery, IEnumerable<Reservation>>
{
    private readonly IReservationRepository _repository;

    public GetReservationHistoryByEquipmentIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Reservation>> Handle(GetReservationHistoryByEquipmentIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetListByEquipmentIdAsync(request.EquipmentId);
    }
}