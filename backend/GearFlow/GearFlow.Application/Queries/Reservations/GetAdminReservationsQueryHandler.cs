using GearFlow.Application.DTOs;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class
    GetAdminReservationsQueryHandler : IRequestHandler<GetAdminReservationsQuery, GetAdminReservationsResponseDto>
{
    private readonly IReservationRepository _repository;

    public GetAdminReservationsQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAdminReservationsResponseDto> Handle(GetAdminReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var history = await _repository.GetAllCompletedReservations();
        var active = await _repository.GetActiveReservationsAsync();
        var pending = await _repository.GetAllPendingReservations();

        return new GetAdminReservationsResponseDto(active, history, pending);
    }
}