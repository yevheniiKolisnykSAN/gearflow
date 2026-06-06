using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using GearFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(GearFlowDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetActiveByUserIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Where(r => r.UserId == id && r.Status == ReservationStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int id)
    {
        return await _context.Reservations.Where(r => r.UserId == id).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetListByEquipmentIdAsync(int id)
    {
        return await _context.Reservations.Where(r => r.EquipmentId == id).ToListAsync();
    }

    public async Task<Reservation?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Reservation?> GetActiveByEquipmentIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.EquipmentId == id && r.Status == ReservationStatus.Active);
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .Where(r => r.Status == ReservationStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetHistoryByUserId(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .Where(r => (r.Status == ReservationStatus.Completed || r.Status == ReservationStatus.Cancelled) && r.UserId == id)
            .ToListAsync();
    }

    public async Task<Reservation?> GetConflictingReservationAsync(int equipmentId, DateTime start, DateTime end)
    {
        return await _context.Reservations
            .FirstOrDefaultAsync(r =>
                r.EquipmentId == equipmentId &&
                r.Status == ReservationStatus.Active &&
                r.StartDate < end &&
                r.EndDate > start);
    }

    public async Task<IEnumerable<DateTime>> GetReservedDates(int equipmentId)
    {
        var reservations = await _context.Reservations
            .Where(r => r.EquipmentId == equipmentId && r.Status == ReservationStatus.Active)
            .Select(r => new { r.StartDate, r.EndDate })
            .ToListAsync();

        var dates = new List<DateTime>();
        foreach (var reservation in reservations)
        {
            var current = reservation.StartDate.Date;
            while (current <= reservation.EndDate.Date)
            {
                dates.Add(current);
                current = current.AddDays(1);
            }
        }

        return dates;
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsListByEquipmentId(int equipmentId)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .Where(r => r.Status == ReservationStatus.Active && r.EquipmentId == equipmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetAllPendingReservations()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .Where(r => r.Status == ReservationStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetAllPendingReservationsByUserId(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .Where(r => r.UserId == id && r.Status == ReservationStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetAllCompletedReservations()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Equipment)
            .Include(r => r.Defect)
            .Where(r => r.Status == ReservationStatus.Completed || r.Status == ReservationStatus.Cancelled)
            .ToListAsync();
    }
}