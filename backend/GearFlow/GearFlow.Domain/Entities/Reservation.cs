using GearFlow.Domain.Enums;

namespace GearFlow.Domain.Entities;

public class Reservation : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? PendingAt { get; set; }


    public ReservationStatus Status { get; set; }
    public User User { get; set; }
    public Equipment Equipment { get; set; }
    public Defect? Defect { get; set; }
}