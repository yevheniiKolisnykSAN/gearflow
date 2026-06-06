using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public class CompleteReservationCommandHandler : IRequestHandler<CompleteReservationCommand, bool>
{
    private readonly IReservationRepository _repository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IRepository<Defect> _defectRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public CompleteReservationCommandHandler(
        IReservationRepository repository,
        IUserRepository userRepository,
        IEquipmentRepository equipmentRepository,
        IRepository<Defect> defectRepository,
        IEmailService emailService,
        IConfiguration configuration
    )
    {
        _repository = repository;
        _equipmentRepository = equipmentRepository;
        _defectRepository = defectRepository;
        _emailService = emailService;
        _configuration = configuration;
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

        if (request.DefectComment != null)
        {
            var defect = new Defect
            {
                Comment = request.DefectComment,
                EquipmentId = equipment.Id,
                ReservationId = reservation.Id,
                UserId = request.UserId,
                Status = DefectStatus.New
            };
            await _defectRepository.AddAsync(defect);
        }

        reservation.PendingAt = DateTime.UtcNow;
        reservation.Status = ReservationStatus.Pending;
        await _repository.UpdateAsync(reservation);

        var adminEmail = _configuration["Email:AdminEmail"]!;
        await _emailService.SendEmailAsync(
            adminEmail,
            "GearFlow - New Return Pending Confirmation",
            $"<h2>New return waiting for confirmation</h2>" +
            $"<p>Equipment: <b>{equipment.Name}</b></p>" +
            $"<p>Reservation ID: <b>{reservation.Id}</b></p>" +
            $"<p>Pending since: <b>{DateTime.UtcNow:dd-MM-yyyy HH:mm}</b></p>"
        );

        return true;
    }
}