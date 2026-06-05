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
        return await _context.Reservations.Where(r => r.Status == ReservationStatus.Active).ToListAsync();
    }
}