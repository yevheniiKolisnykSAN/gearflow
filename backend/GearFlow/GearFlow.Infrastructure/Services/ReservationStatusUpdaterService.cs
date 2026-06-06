using GearFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GearFlow.Infrastructure.Services;

public class ReservationStatusUpdaterService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationStatusUpdaterService> _logger;

    public ReservationStatusUpdaterService(
        IServiceScopeFactory scopeFactory,
        ILogger<ReservationStatusUpdaterService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateReservationStatuses();
            
            var now = DateTime.UtcNow;
            var nextMidnight = now.Date.AddDays(1);
            var delay = nextMidnight - now;
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task UpdateReservationStatuses()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GearFlowDbContext>();

        var today = DateTime.UtcNow.Date;

        var reservations = await context.Reservations
            .Include(r => r.Equipment)
            .Where(r => r.Status == Domain.Enums.ReservationStatus.Active &&
                        r.StartDate.Date <= today &&
                        r.Equipment.StatusId == 1)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            reservation.Equipment.StatusId = 2;
            _logger.LogInformation($"Equipment {reservation.EquipmentId} marked as Reserved");
        }

        await context.SaveChangesAsync();
        
        var emailService = scope.ServiceProvider.GetRequiredService<GearFlow.Domain.Interfaces.IEmailService>();

        var tomorrow = today.AddDays(1);
        var endingTomorrow = await context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Where(r => r.Status == Domain.Enums.ReservationStatus.Active &&
                        r.EndDate.Date == tomorrow)
            .ToListAsync();

        foreach (var reservation in endingTomorrow)
        {
            await emailService.SendEmailAsync(
                reservation.User.Email,
                "GearFlow - Return Reminder",
                $"<h2>Please return equipment tomorrow!</h2>" +
                $"<p>Equipment: <b>{reservation.Equipment.Name}</b></p>" +
                $"<p>Return by: <b>{reservation.EndDate:dd-MM-yyyy}</b></p>"
            );
            _logger.LogInformation($"Reminder sent for reservation {reservation.Id}");
        }
    }
}