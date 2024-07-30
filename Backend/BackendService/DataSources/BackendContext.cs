using BackendService.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.DataSources;

public class BackendContext : DbContext
{
    public DbSet<Network> Networks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<NetworkUser> NetworkUsers { get; set; }
    public DbSet<NetworkDevice> NetworkDevices { get; set; }
    public DbSet<NetworkUsersGroup> NetworkUsersGroups { get; set; }
    public DbSet<NetworkDevicesGroup> NetworkDevicesGroups { get; set; }
    public DbSet<NetworkDevicesAccess> NetworkDevicesAccesses { get; set; }

    private IConfiguration Configuration { get; set; }

    public BackendContext(IConfiguration configuration)
    {
        Configuration = configuration;
        //Database.EnsureCreated();
        //Database.EnsureDeleted();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Configuration["ConnectionString"];

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.LogTo(Console.WriteLine);
    }
}