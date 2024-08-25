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

    private ILogger Logger { get; set; }
    private IConfiguration Configuration { get; set; }

    public BackendContext(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        Logger = loggerFactory.CreateLogger<BackendContext>();
        Configuration = configuration;

        Logger.LogDebug("Подключение к базе.");

        Database.EnsureCreated();

        Logger.LogDebug("Подключение к базе прошло успешно.");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Logger.LogDebug("Конфигурация подключения к базе.");

        var connectionString = Configuration["ConnectionString"];

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.LogTo(Console.WriteLine);

        Logger.LogDebug("Конфигурация подключения к базе прошла успешно.");
    }
}