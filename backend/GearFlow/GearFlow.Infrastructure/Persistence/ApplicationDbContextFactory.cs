using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GearFlow.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<GearFlowDbContext>
{
    public GearFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GearFlowDbContext>();
        
        optionsBuilder.UseNpgsql("Host=localhost;Database=gearflow;Username=postgres;Password=postgres");

        return new GearFlowDbContext(optionsBuilder.Options);
    }
}