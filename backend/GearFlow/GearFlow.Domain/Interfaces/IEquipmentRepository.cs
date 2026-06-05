using GearFlow.Domain.Entities;
using GearFlow.Domain.Models;

namespace GearFlow.Domain.Interfaces;

public interface IEquipmentRepository
{
    Task<IEnumerable<Equipment>> GetEquipmentList(
        string? search = null,
        string? name = null,
        string? serialNumber = null,
        string? specification = null,
        int[]? maxLoanDays = null,
        int[]? locationId = null,
        int[]? statusId = null,
        int[]? typeId = null);
    public Task<Equipment> GetByIdAsync(int id);
}