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

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int id)
    {
        return await _context.Reservations.Where(r => r.UserId == id).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByEquipmentIdAsync(int id)
    {
        return await _context.Reservations.Where(r => r.EquipmentId == id).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
    {
        return await _context.Reservations.Where(r => r.Status == ReservationStatus.Active).ToListAsync();
    }
}