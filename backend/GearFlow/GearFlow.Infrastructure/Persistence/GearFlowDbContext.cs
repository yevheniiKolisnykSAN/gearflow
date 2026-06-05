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
        
        modelBuilder.Entity<EquipmentStatus>().HasData(
            new EquipmentStatus { Id = 1, Name = "Available" },
            new EquipmentStatus { Id = 2, Name = "Reserved" },
            new EquipmentStatus { Id = 3, Name = "Borrowed" },
            new EquipmentStatus { Id = 4, Name = "Serviced" },
            new EquipmentStatus { Id = 5, Name = "Destroyed" }
        );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentType> EquipmentTypes { get; set; }
    public DbSet<EquipmentStatus> EquipmentStatuses { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}