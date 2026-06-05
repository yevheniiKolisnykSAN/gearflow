using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using GearFlow.Domain.Models;
using GearFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Repositories;

public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
{
    private readonly GearFlowDbContext _context;

    public EquipmentRepository(GearFlowDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentList(
        string? search = null,
        string? name = null,
        string? serialNumber = null,
        string? specification = null,
        int[]? maxLoanDays = null,
        int[]? locationId = null,
        int[]? statusId = null,
        int[]? typeId = null)
    {
        var query = _context.Equipments
            .Include(e => e.Location)
            .Include(e => e.Type)
            .Include(e => e.Status)
            .Where(e => !e.Archived);

        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(e =>
                e.Name.ToLower().Contains(searchLower) ||
                e.Specification.ToLower().Contains(searchLower) ||
                e.SerialNumber.ToLower().Contains(searchLower) ||
                e.Location.Name.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrEmpty(name))
            query = query.Where(e => e.Name.Contains(name));

        if (!string.IsNullOrEmpty(serialNumber))
            query = query.Where(e => e.SerialNumber.Contains(serialNumber));

        if (!string.IsNullOrEmpty(specification))
            query = query.Where(e => e.Specification.Contains(specification));

        if (maxLoanDays != null && maxLoanDays.Any())
            query = query.Where(e => maxLoanDays.Contains(e.MaxLoanDays));

        if (locationId != null && locationId.Any())
            query = query.Where(e => locationId.Contains(e.LocationId));

        if (statusId != null && statusId.Any())
            query = query.Where(e => statusId.Contains(e.StatusId));

        if (typeId != null && typeId.Any())
            query = query.Where(e => typeId.Contains(e.TypeId));

        return await query.ToListAsync();
    }

    public async Task<Equipment> GetByIdAsync(int id)
    {
        return await _context.Equipments
            .Include(e => e.Location)
            .Include(e => e.Type)
            .Include(e => e.Status)
            .FirstOrDefaultAsync(e => e.Id == id && !e.Archived);
    }
}