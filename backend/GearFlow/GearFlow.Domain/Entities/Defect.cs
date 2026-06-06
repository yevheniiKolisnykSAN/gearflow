using GearFlow.Domain.Enums;

namespace GearFlow.Domain.Entities;

public class Defect : BaseEntity
{
    public string Comment { get; set; }
    public DefectStatus Status { get; set; }
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public int? ReservationId { get; set; }

    public User User { get; set; }
    public Equipment Equipment { get; set; }
    public Reservation? Reservation { get; set; }
}