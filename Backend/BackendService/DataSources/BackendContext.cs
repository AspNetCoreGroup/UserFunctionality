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
    public DbSet<NetworkRule> NetworkRules { get; set; }

    private IConfiguration Configuration { get; set; }

    public BackendContext(IConfiguration configuration)
    {
        Configuration = configuration;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Configuration["ConnectionString"];

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.LogTo(Console.WriteLine);
    }
}