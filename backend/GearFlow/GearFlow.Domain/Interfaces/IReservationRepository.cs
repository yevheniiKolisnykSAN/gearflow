using GearFlow.Domain.Entities;

namespace GearFlow.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetActiveByUserIdAsync(int id);
    Task<IEnumerable<Reservation>> GetByUserIdAsync(int id);
    Task<IEnumerable<Reservation>> GetListByEquipmentIdAsync(int id);
    
    Task<Reservation?> GetByIdWithDetailsAsync(int id);
    Task<Reservation?> GetActiveByEquipmentIdAsync(int id);
    Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
    Task<IEnumerable<Reservation>> GetHistoryByUserId(int id);
    Task<Reservation?> GetConflictingReservationAsync(int equipmentId, DateTime start, DateTime end);
    Task<IEnumerable<DateTime>> GetReservedDates(int equipmentId);
    Task<IEnumerable<Reservation>> GetActiveReservationsListByEquipmentId(int equipmentId);
    Task<IEnumerable<Reservation>> GetAllPendingReservations();
    Task<IEnumerable<Reservation>> GetAllPendingReservationsByUserId(int id);
    Task<IEnumerable<Reservation>> GetAllCompletedReservations();
}