namespace GearFlow.Domain.Entities;

public class Notification : BaseEntity
{
    public string Message { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
    public int ReservationId { get; set; }

    public User User { get; set; }
    public Reservation Reservation { get; set; }
}