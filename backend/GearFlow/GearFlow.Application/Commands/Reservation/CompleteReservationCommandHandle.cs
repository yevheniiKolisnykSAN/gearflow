using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public class CompleteReservationCommandHandle : IRequestHandler<CompleteReservationCommand, bool>
{
    private readonly IReservationRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public CompleteReservationCommandHandle(
        IReservationRepository repository,
        IUserRepository userRepository,
        IEquipmentRepository equipmentRepository
    )
    {
        _repository = repository;
        _userRepository = userRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<bool> Handle(CompleteReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.ReservationId);
        if (reservation == null)
        {
            throw new KeyNotFoundException("Reservation not found");
        }

        if (request.UserRoleId == 2 && reservation.UserId != request.UserId)
        {
            throw new InvalidOperationException("Only user can complete reservation");
        }

        var equipment = await _equipmentRepository.GetByIdAsync(reservation.EquipmentId);
        if (equipment == null)
        {
            throw new KeyNotFoundException("Equipment not found");
        }

        reservation.Status = ReservationStatus.Completed;
        await _repository.UpdateAsync(reservation);
        equipment.StatusId = 1;
        await _equipmentRepository.UpdateAsync(equipment);
        return true;
    }
}