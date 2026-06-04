using GearFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Persistence;

public class GearFlowDbContext : DbContext
{
    public GearFlowDbContext(DbContextOptions<GearFlowDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Employee" }
        );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentType> EquipmentTypes { get; set; }
    public DbSet<EquipmentStatus> EquipmentStatuses { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}