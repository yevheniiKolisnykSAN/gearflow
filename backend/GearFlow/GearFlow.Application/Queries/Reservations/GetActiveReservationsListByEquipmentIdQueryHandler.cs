using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class
    GetActiveReservationsListByEquipmentIdQueryHandler : IRequestHandler<GetActiveReservationsListByEquipmentIdQuery,
    IEnumerable<Reservation>>
{
    private readonly IReservationRepository _repository;

    public GetActiveReservationsListByEquipmentIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Reservation>> Handle(GetActiveReservationsListByEquipmentIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetActiveReservationsListByEquipmentId(request.id);
    }
}