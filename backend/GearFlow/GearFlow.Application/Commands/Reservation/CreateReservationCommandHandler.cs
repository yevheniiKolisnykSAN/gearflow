using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, int>
{
    private readonly IReservationRepository _repository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public CreateReservationCommandHandler(
        IReservationRepository repository,
        IEquipmentRepository equipmentRepository,
        IUserRepository userRepository,
        IEmailService emailService
        )
    {
        _repository = repository;
        _equipmentRepository = equipmentRepository;
        _userRepository = userRepository;
        _emailService = emailService;
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
        
        var conflicting = await _repository.GetConflictingReservationAsync(
            request.EquipmentId, 
            request.StartDate, 
            request.EndDate);

        if (conflicting != null)
        {
            throw new InvalidOperationException("Equipment already reserved for this period");
        }
        
        if (request.StartDate.Date <= DateTime.UtcNow.Date)
        {
            equipment.StatusId = 2;
            await _equipmentRepository.UpdateAsync(equipment);
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
        
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "GearFlow - Reservation Confirmed",
                $"<h2>Your reservation is confirmed!</h2>" +
                $"<p>Equipment: <b>{equipment.Name}</b></p>" +
                $"<p>From: <b>{request.StartDate:dd-MM-yyyy}</b></p>" +
                $"<p>To: <b>{request.EndDate:dd-MM-yyyy}</b></p>"
            );
        }
        
        return newReservation.Id;
    }
}