using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public class ConfirmReturnCommandHandler : IRequestHandler<ConfirmReturnCommand, bool>
{
    private readonly IReservationRepository _repository;
    private readonly IRepository<Defect> _defectRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public ConfirmReturnCommandHandler(
        IReservationRepository repository,
        IRepository<Defect> defectRepository,
        IEquipmentRepository equipmentRepository,
        IEmailService emailService,
        IUserRepository userRepository
    )
    {
        _repository = repository;
        _defectRepository = defectRepository;
        _equipmentRepository = equipmentRepository;
        _emailService = emailService;
        _userRepository = userRepository;
    }


    public async Task<bool> Handle(ConfirmReturnCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdWithDetailsAsync(request.ReservationId);

        if (reservation == null)
        {
            throw new KeyNotFoundException("Reservation not found");
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new InvalidOperationException("Reservation should have \"Pending\" status");
        }

        var equipment = await _equipmentRepository.GetByIdAsync(reservation.EquipmentId);
        if (equipment == null)
        {
            throw new KeyNotFoundException("Equipment not found");
        }

        if (request.DefectComment != null)
        {
            if (reservation.Defect != null)
            {
                reservation.Defect.Comment = request.DefectComment;
                await _defectRepository.UpdateAsync(reservation.Defect);
            }
            else
            {
                var defect = new Defect
                {
                    Comment = request.DefectComment,
                    EquipmentId = equipment.Id,
                    ReservationId = reservation.Id,
                    UserId = reservation.UserId,
                    Status = DefectStatus.New
                };
                await _defectRepository.AddAsync(defect);
            }
        }


        reservation.Status = ReservationStatus.Completed;
        reservation.CompletedAt = DateTime.UtcNow;
        equipment.StatusId = 1;

        await _repository.UpdateAsync(reservation);
        await _equipmentRepository.UpdateAsync(equipment);
        
        var user = await _userRepository.GetByIdAsync(reservation.UserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "GearFlow - Return Confirmed",
                $"<h2>Your return has been confirmed!</h2>" +
                $"<p>Equipment: <b>{equipment.Name}</b></p>" +
                $"<p>Completed at: <b>{DateTime.UtcNow:dd-MM-yyyy HH:mm}</b></p>"
            );
        }
        
        return true;
    }
}