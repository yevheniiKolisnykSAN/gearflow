namespace GearFlow.Domain.Entities;

public class Equipment : BaseEntity
{
    public string SerialNumber { get; set; }
    public string Location { get; set; }
    public string Specification { get; set; }
    public int MaxLoanDays { get; set; }
    public int StatusId { get; set; }
    public int TypeId { get; set; }

    public EquipmentStatus Status { get; set; }
    public EquipmentType Type { get; set; }
}