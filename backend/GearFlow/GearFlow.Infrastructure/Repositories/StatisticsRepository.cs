using GearFlow.Application.DTOs;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using GearFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly GearFlowDbContext _context;

    public StatisticsRepository(GearFlowDbContext context)
    {
        _context = context;
    }

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        var totalReservations = await _context.Reservations.CountAsync();

        var activeReservations = await _context.Reservations
            .CountAsync(r => r.Status == ReservationStatus.Active);

        var completedReservations = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Completed)
            .ToListAsync();

        var avgDuration = completedReservations.Any()
            ? completedReservations.Average(r => (r.EndDate - r.StartDate).Days)
            : 0;

        var totalDefects = await _context.Defects.CountAsync();

        var reservationsWithEquipment = await _context.Reservations
            .Include(r => r.Equipment)
            .ThenInclude(e => e.Type)
            .ToListAsync();

        var topEquipment = reservationsWithEquipment
            .GroupBy(r => r.Equipment.Name)
            .Select(g => new TopEquipmentDto(g.Key, g.Count()))
            .OrderByDescending(e => e.Count)
            .Take(5)
            .ToList();

        var byType = reservationsWithEquipment
            .GroupBy(r => r.Equipment.Type.Name)
            .Select(g => new TypeStatDto(g.Key, g.Count()))
            .ToList();

        var currentYear = DateTime.UtcNow.Year;
        var reservationsThisYear = await _context.Reservations
            .Where(r => r.StartDate.Year == currentYear)
            .ToListAsync();

        var byMonth = reservationsThisYear
            .GroupBy(r => r.StartDate.Month)
            .Select(g => new MonthStatDto(g.Key.ToString(), g.Count()))
            .OrderBy(m => m.Month)
            .ToList();

        return new StatisticsDto(
            totalReservations,
            activeReservations,
            avgDuration,
            totalDefects,
            topEquipment,
            byMonth,
            byType);
    }
}