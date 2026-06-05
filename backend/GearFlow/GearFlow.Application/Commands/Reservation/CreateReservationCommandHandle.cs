using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public class CreateReservationCommandHandle : IRequestHandler<CreateReservationCommand, int>
{
    private readonly IReservationRepository _repository;
    private readonly IEquipmentRepository _equipmentRepository;

    public CreateReservationCommandHandle(IReservationRepository repository, IEquipmentRepository equipmentRepository)
    {
        _repository = repository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<int> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId);
        if (equipment == null)
        {
            throw new KeyNotFoundException("Equipment not found");
        }

        var days = (request.EndDate - request.StartDate).Days;
        if (days > equipment.MaxLoanDays)
        {
            throw new InvalidOperationException($"Max loan days is {equipment.MaxLoanDays}");
        }

        var activeReservations = await _repository.GetActiveByEquipmentIdAsync(request.EquipmentId);
        if (activeReservations != null)
        {
            throw new InvalidOperationException("Equipment already reserved.");
        }

        var newReservation = new Domain.Entities.Reservation
        {
            EquipmentId = request.EquipmentId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            UserId = request.UserId,
            Status = ReservationStatus.Active
        };

        await _repository.AddAsync(newReservation);
        
        equipment.StatusId = 2; 
        await _equipmentRepository.UpdateAsync(equipment);
        
        return newReservation.Id;
    }
}