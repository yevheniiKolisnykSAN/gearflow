using GearFlow.Domain.Entities;

namespace GearFlow.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByUserIdAsync(int id);
    Task<IEnumerable<Reservation>> GetByEquipmentIdAsync(int id);
    Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
}